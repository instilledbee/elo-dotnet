using System;
using System.Collections.Generic;
using System.Linq;
using EloDotNet.Interfaces;
using EloDotNet.Enums;

namespace EloDotNet
{
    /// <summary>
    /// Default implementation of <see cref="IRankingSystem{TMatch, TPlayer}"/>, using <see cref="Match"/> to record matches, and <see cref="Player"/> to refer to participants.
    /// </summary>
    public class RankingSystem : IRankingSystem<Match, Player>
    {
        public ICollection<Player> Players { get; }
        public ICollection<Match> Matches { get; }
        public double StartingElo { get; }
        public double KFactor { get; }

        private const double WINNER_SCORE = 1.0f;
        private const double DRAW_SCORE = 0.5f;
        private const double LOSER_SCORE = 0.0f;

        /// <summary>
        /// Stores the expected Elo of a player after N number of matches in the RankingSystem
        /// </summary>
        private readonly Dictionary<(Guid, int), double> _eloCache;

        public RankingSystem(double startingElo = 1000, double kFactor = 400)
        {
            Players = new List<Player>();
            Matches = new List<Match>();

            StartingElo = startingElo;
            KFactor = kFactor;
            _eloCache = new Dictionary<(Guid, int), double>();
        }

        public void RegisterPlayer(Player player)
        {
            Players.Add(player);
            _eloCache[(player.Id, 0)] = StartingElo;
        }

        public void RecordMatch(Player playerA, Player playerB, MatchWinner winner = MatchWinner.Draw)
        {
            if (!Players.Any(p => p.Id == playerA.Id))
                throw new ArgumentException($"Winning player {playerA.Id} is not in this RankingSystem.");

            if (!Players.Any(p => p.Id == playerB.Id))
                throw new ArgumentException($"Losing player {playerB.Id} is not in this RankingSystem.");

            Matches.Add(new Match(playerA, playerB, winner));
        }

        public double CalculateElo(Player player)
        {
            if (!Players.Any(p => p.Id == player.Id))
                throw new ArgumentException($"Player {player.Id} is not in this RankingSystem.");

            // no matches played = player only has starting Elo
            if (!Matches.Any(m => m.Winner?.Id == player.Id || m.Loser?.Id == player.Id))
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

                    if (p.Id == playerAId)
                        newElo += eloChangeA;
                    else if (p.Id == playerBId)
                        newElo += eloChangeB;

                    _eloCache[(p.Id, matchIndex)] = newElo;
                }
            }

            return _eloCache[(player.Id, matchIndex)];
        }
    }
}
