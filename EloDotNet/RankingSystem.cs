using System;
using System.Collections.Generic;
using System.Linq;
using EloDotNet.Interfaces;
using EloDotNet.Enums;

namespace EloDotNet
{
    /// <summary>
    /// Default generic implementation of <see cref="IRankingSystem{TMatch, TPlayer}"/>, allowing custom implementations for match and player entities.
    /// </summary>
    public class RankingSystem<TMatch, TPlayer, TIndex, TId> : IRankingSystem<TMatch, TPlayer>
        where TMatch : IMatch<TPlayer, TIndex>, new()
        where TPlayer : IPlayer<TId>
        where TIndex : IComparable<TIndex>
        where TId : IEquatable<TId>
    {
        public ICollection<TPlayer> Players { get; }
        public ICollection<TMatch> Matches { get; }
        public double StartingElo { get; }
        public double KFactor { get; }

        private const double WINNER_SCORE = 1.0f;
        private const double DRAW_SCORE = 0.5f;
        private const double LOSER_SCORE = 0.0f;

        /// <summary>
        /// Stores the expected Elo of a player after N number of matches in the RankingSystem
        /// </summary>
        private readonly Dictionary<(TId, int), double> _eloCache;

        public RankingSystem(double startingElo, double kFactor)
        {
            Players = new List<TPlayer>();
            Matches = new List<TMatch>();

            StartingElo = startingElo;
            KFactor = kFactor;
            _eloCache = new Dictionary<(TId, int), double>();
        }

        public void RegisterPlayer(TPlayer player)
        {
            Players.Add(player);
            _eloCache[(player.Id, 0)] = StartingElo;
        }

        public void RecordMatch(TPlayer playerA, TPlayer playerB, MatchWinner result = MatchWinner.Draw)
        {
            if (!Players.Any(p => p.Id.Equals(playerA.Id)))
                throw new ArgumentException($"Winning player {playerA.Id} is not in this RankingSystem.");

            if (!Players.Any(p => p.Id.Equals(playerB.Id)))
                throw new ArgumentException($"Losing player {playerB.Id} is not in this RankingSystem.");

            TPlayer winner = default;
            TPlayer loser = default;

            switch (result)
            {
                case MatchWinner.PlayerA:
                    winner = playerA;
                    loser = playerB; 
                    break;

                case MatchWinner.PlayerB:
                    winner = playerB;
                    loser = playerA;
                    break;
            }

            TMatch match = new TMatch()
            {
                PlayerA = playerA,
                PlayerB = playerB,
                Result = result,
            };

            Matches.Add(match);
        }

        public double CalculateElo(TPlayer player)
        {
            if (!Players.Any(p => p.Id.Equals(player.Id)))
                throw new ArgumentException($"Player {player.Id} is not in this RankingSystem.");

            // no matches played = player only has starting Elo
            if (!Matches.Any(m => (m.Winner?.Id.Equals(player.Id) ?? false) || (m.Loser?.Id.Equals(player.Id) ?? false)))
                return StartingElo;

            if (_eloCache.ContainsKey((player.Id, Matches.Count)))
                return _eloCache[(player.Id, Matches.Count)];

            var orderedMatches = Matches.OrderBy(m => m.RecordIndex);
            int matchIndex = 0;

            // Simulate each match in the history
            // This calculate's each player's Elo as each historical match progresses
            foreach (var match in orderedMatches)
            {
                matchIndex++;

                var playerAId = match.PlayerA.Id;
                var playerBId = match.PlayerB.Id;

                // take winner's/loser's latest Elo ratings
                var prevMatchIndex = Math.Max(0, matchIndex - 1);
                var currentRatingA = _eloCache[(playerAId, prevMatchIndex)];
                var currentRatingB = _eloCache[(playerBId, prevMatchIndex)];

                // 1. Take the difference in ratings
                var deltaA = currentRatingB - currentRatingA;
                var deltaB = currentRatingA - currentRatingB;

                // 2. Divide the difference in ratings by 400
                // 3. Find the value of ten to the power of this fraction and add 1
                // 4. The expected score is the multiplicative inverse of the latter
                var expectedScoreA = 1f / (Math.Pow(10, deltaA / 400f) + 1);
                var expectedScoreB = 1f / (Math.Pow(10, deltaB / 400f) + 1);

                // 5. use K-factor and the final score to get the Elo rating change.
                double eloChangeA = 0f, eloChangeB = 0f;

                switch (match.Result)
                {
                    case MatchWinner.PlayerA:
                        eloChangeA = KFactor * (WINNER_SCORE - expectedScoreA);
                        eloChangeB = KFactor * (LOSER_SCORE - expectedScoreB);
                        break;

                    case MatchWinner.PlayerB:
                        eloChangeA = KFactor * (LOSER_SCORE - expectedScoreA);
                        eloChangeB = KFactor * (WINNER_SCORE - expectedScoreB);
                        break;

                    case MatchWinner.Draw:
                        eloChangeA = KFactor * (DRAW_SCORE - expectedScoreA);
                        eloChangeB = KFactor * (DRAW_SCORE - expectedScoreB);
                        break;
                }

                // Calculate each player's Elo rating for the current match index
                foreach (var p in Players)
                {
                    // by default, if the player didn't participate in the current match, 
                    // just carry over their previous Elo rating
                    double newElo = _eloCache[(p.Id, prevMatchIndex)];

                    if (p.Id.Equals(playerAId))
                        newElo += eloChangeA;
                    else if (p.Id.Equals(playerBId))
                        newElo += eloChangeB;

                    _eloCache[(p.Id, matchIndex)] = newElo;
                }
            }

            return _eloCache[(player.Id, matchIndex)];
        }
    }

    /// <summary>
    /// Default implementation if <see cref="RankingSystem{TMatch, TPlayer, TIndex, TId}"/>, supporting the default <see cref="Match"/> and <see cref="Player"/> types.
    /// </summary>
    public class RankingSystem : RankingSystem<Match, Player, DateTimeOffset, Guid>
    {
        /// <summary>
        /// Initializes a new RankingSystem instance, optionally specifying the starting Elo and k-factor values to use for calculations.
        /// </summary>
        /// <param name="startingElo">The starting Elo for a new player in the system. Defaults to 1000.</param>
        /// <param name="kFactor">The k-factor to use for calculating Elo changes. Defaults to 400.</param>
        public RankingSystem(double startingElo = 1000, double kFactor = 400) : base(startingElo, kFactor) { }
    }
}
