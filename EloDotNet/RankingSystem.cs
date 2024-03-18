using System;
using System.Collections.Generic;
using System.Linq;

namespace EloDotNet
{
    public class RankingSystem
    {
        private readonly ICollection<Player> _players;
        private readonly ICollection<Match> _matches;
        private readonly double _startingElo;
        private readonly double _kFactor;

        private const double WINNER_SCORE = 1.0f;
        private const double DRAW_SCORE = 0.5f;
        private const double LOSER_SCORE = 0.0f;

        /// <summary>
        /// Stores the expected Elo of a player after N number of matches in the RankingSystem
        /// </summary>
        private readonly Dictionary<(Guid, int), double> _eloCache;

        public RankingSystem(double startingElo = 1000, double kFactor = 400)
        {
            _players = new List<Player>();
            _matches = new List<Match>();

            _startingElo = startingElo;
            _kFactor = kFactor;
            _eloCache = new Dictionary<(Guid, int), double>();
        }

        public void RegisterPlayer(Player player)
        {
            _players.Add(player);
            _eloCache[(player.Id, 0)] = _startingElo;
        }

        public void RecordMatch(Player playerA, Player playerB, MatchWinner winner = MatchWinner.Draw)
        {
            if (!_players.Any(p => p.Id == playerA.Id))
                throw new ArgumentException($"Winning player {playerA.Id} is not in this RankingSystem.");

            if (!_players.Any(p => p.Id == playerB.Id))
                throw new ArgumentException($"Losing player {playerB.Id} is not in this RankingSystem.");

            _matches.Add(new Match(playerA, playerB, winner));
        }

        public double CalculateElo(Player player)
        {
            if (!_players.Any(p => p.Id == player.Id))
                throw new ArgumentException($"Player {player.Id} is not in this RankingSystem.");

            // no matches played = player only has starting Elo
            if (!_matches.Any(m => m.Winner?.Id == player.Id || m.Loser?.Id == player.Id))
                return _startingElo;

            if (_eloCache.ContainsKey((player.Id, _matches.Count)))
                return _eloCache[(player.Id, _matches.Count)];

            var orderedMatches = _matches.OrderBy(m => m.RecordTime);
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
                        eloChangeA = _kFactor * (WINNER_SCORE - expectedScoreA);
                        eloChangeB = _kFactor * (LOSER_SCORE - expectedScoreB);
                        break;

                    case MatchWinner.PlayerB:
                        eloChangeA = _kFactor * (LOSER_SCORE - expectedScoreA);
                        eloChangeB = _kFactor * (WINNER_SCORE - expectedScoreB);
                        break;

                    case MatchWinner.Draw:
                        eloChangeA = _kFactor * (DRAW_SCORE - expectedScoreA);
                        eloChangeB = _kFactor * (DRAW_SCORE - expectedScoreB);
                        break;
                }

                // Calculate each player's Elo rating for the current match index
                foreach (var p in _players)
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

        private class PlayerRunningStats
        {
            public Guid PlayerId { get; }
            public int Wins { get; private set; }
            public int Losses { get; private set; }
            public int Matches => Wins + Losses;
            public List<double> OpponentElos { get; private set; }
            private readonly double _startingElo;
            private readonly double _advantageRate;

            public PlayerRunningStats(Guid playerId, double startingElo, double advantageRate)
            {
                PlayerId = playerId;
                OpponentElos = new List<double>();
                _startingElo = startingElo;
                _advantageRate = advantageRate;
            }

            public void AddWin(double opponentElo)
            {
                Wins++;
                OpponentElos.Add(opponentElo);
            }

            public void AddLoss(double opponentElo)
            {
                Losses++;
                OpponentElos.Add(opponentElo);
            }

            public double CalculateElo()
            {
                if (Matches == 0) return _startingElo;
                return (OpponentElos.Sum() + (_advantageRate * (Wins - Losses))) / Matches;
            }
        }
    }
}
