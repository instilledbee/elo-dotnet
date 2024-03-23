using System;
using EloDotNet.Enums;
using EloDotNet.Interfaces;

namespace EloDotNet
{
    /// <summary>
    /// Default <see cref="IMatch{TPlayer}"/> implementation, whose participants are two <see cref="Player"/> entities.
    /// </summary>
    public class Match : IMatch<Player, DateTimeOffset>
    {
        public Player PlayerA { get; }
        public Player PlayerB { get; }

        public Player Winner
        {
            get
            {
                switch (Result)
                {
                    case MatchWinner.PlayerA:
                        return PlayerA;
                    case MatchWinner.PlayerB:
                        return PlayerB;
                    default:
                        return null;
                }
            }
        }

        public Player Loser
        {
            get
            {
                switch (Result)
                {
                    case MatchWinner.PlayerA:
                        return PlayerB;
                    case MatchWinner.PlayerB:
                        return PlayerA;
                    default:
                        return null;
                }
            }
        }

        public MatchWinner Result { get; }
        public DateTimeOffset RecordIndex { get; }

        public Match(Player playerA, Player playerB, MatchWinner winner = MatchWinner.Draw)
        {
            if (playerA == null)
                throw new ArgumentNullException(nameof(playerA));

            if (playerB == null)
                throw new ArgumentNullException(nameof(playerB));

            if (playerA == playerB ||
                playerA.Id == playerB.Id)
                throw new ArgumentException("Player A and B cannot be the same player.");

            PlayerA = playerA;
            PlayerB = playerB;
            RecordIndex = DateTimeOffset.UtcNow;
            Result = winner;
        }

        public bool IncludesPlayer(Guid playerId) => PlayerA.Id == playerId || PlayerB.Id == playerId;
    }
}
