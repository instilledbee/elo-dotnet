using System;

namespace EloDotNet
{
    public interface IMatch
    {
        Tuple<IPlayer, IPlayer> Players { get; set; }
        IPlayer Winner { get; set; }
        IPlayer Loser { get; set; }
    }

    public class Match
    {
        public Player PlayerA { get; }
        public Player PlayerB { get; }

        public Player Winner
        {
            get
            {
                switch (this.Result)
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
                switch (this.Result)
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
        public DateTimeOffset RecordTime { get; }

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
            RecordTime = DateTimeOffset.UtcNow;
            Result = winner;
        }

        public bool IncludesPlayer(Guid playerId) => PlayerA.Id == playerId || PlayerB.Id == playerId;
    }

    public enum MatchWinner
    {
        PlayerA,
        PlayerB,
        Draw
    }
}
