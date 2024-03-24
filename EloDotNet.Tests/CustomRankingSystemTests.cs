using EloDotNet.Enums;
using EloDotNet.Interfaces;

namespace EloDotNet.Tests
{
    /// <summary>
    /// Tests to determine the usability of the generic interfaces in EloDotNet.
    /// </summary>
    public class CustomRankingSystemTests
    {
        [Fact]
        public void CustomRankingSystem_CalculatesElo()
        {
            // arrange
            var sut = new CustomRankingSystem();
            var p1 = new CustomPlayer();
            var p2 = new CustomPlayer();

            // act
            sut.RegisterPlayer(p1);
            sut.RegisterPlayer(p2);

            sut.RecordMatch(p1, p2, MatchWinner.PlayerA);
            sut.RecordMatch(p1, p2, MatchWinner.PlayerA);
            sut.RecordMatch(p2, p1, MatchWinner.PlayerA);

            // assert
            Assert.Equal(1050f, sut.CalculateElo(p1));
            Assert.Equal(950f, sut.CalculateElo(p2));
        }
    }

    internal class CustomPlayer : IPlayer<int>
    {

        internal static int LastId = 1;

        public int Id { get; }

        internal CustomPlayer() { this.Id = LastId++; }
    }

    internal class CustomMatch : IMatch<CustomPlayer, int>
    {
        public CustomPlayer PlayerA { get; set; }

        public CustomPlayer PlayerB { get; set; }

        public MatchWinner Result { get; set; } = MatchWinner.Draw;

        public int RecordIndex { get; set; }

        public CustomPlayer Winner { get; }

        public CustomPlayer Loser { get; }

        internal static int LastRecordIndex = 1;

        internal CustomMatch(CustomPlayer winner, CustomPlayer loser)
        {
            RecordIndex = CustomMatch.LastRecordIndex++;

            PlayerA = winner;
            Winner = winner;

            PlayerB = loser;
            Loser = loser;

            if (PlayerA.Id == winner.Id)
                Result = MatchWinner.PlayerA;

            else if (PlayerB.Id == winner.Id)
                Result = MatchWinner.PlayerB;
        }
    }

    internal class CustomRankingSystem : IRankingSystem<CustomMatch, CustomPlayer>
    {
        public double StartingElo => 1000;

        public double KFactor => 50;

        public ICollection<CustomMatch> Matches { get; }

        public ICollection<CustomPlayer> Players { get; }

        public CustomRankingSystem()
        {
            Matches = new List<CustomMatch>();
            Players = new List<CustomPlayer>();
        }

        public double CalculateElo(CustomPlayer player)
        {
            // Mock implementation to illustrate a custom Elo ranking system
            // Not meant for actual integrations!
            var victoriesInSystem = Matches.Count(m => m.Winner.Id == player.Id);
            var lossesInSystem = Matches.Count(m => m.Loser.Id == player.Id);

            return StartingElo + (victoriesInSystem * KFactor) - (lossesInSystem * KFactor);
        }

        public void RecordMatch(CustomPlayer playerA, CustomPlayer playerB, MatchWinner winner)
        {
            CustomMatch match = null;

            switch (winner)
            {
                case MatchWinner.PlayerA:
                    match = new CustomMatch(playerA, playerB);
                    break;

                case MatchWinner.PlayerB:
                    match = new CustomMatch(playerB, playerA);
                    break;

                case MatchWinner.Draw:
                    throw new ArgumentException("CustomMatches don't support draws.");
            }

            Matches.Add(match);
        }

        public void RegisterPlayer(CustomPlayer player)
        {
            Players.Add(player);
        }
    }
}
