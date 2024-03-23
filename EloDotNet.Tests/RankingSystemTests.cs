using EloDotNet.Enums;

namespace EloDotNet.Tests
{
    public class RankingSystemTests
    {
        [Fact]
        public void RankingSystem_CalculateElo_WithDefaultSettings()
        {
            var p1 = new Player();
            var p2 = new Player();
            var p3 = new Player();

            RankingSystem sut = new RankingSystem();
            sut.RegisterPlayer(p1);
            sut.RegisterPlayer(p2);
            sut.RegisterPlayer(p3);

            sut.RecordMatch(p1, p2, MatchWinner.PlayerA);

            Assert.Equal(1200f, sut.CalculateElo(p1), 1);
            Assert.Equal(800f, sut.CalculateElo(p2), 1);
            Assert.Equal(1000f, sut.CalculateElo(p3), 1);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(500)]
        public void RankingSystem_CalculateElo_WhenDraw_RetainsElo(double startingElo)
        {
            var p1 = new Player();
            var p2 = new Player();

            RankingSystem sut = new RankingSystem(startingElo);
            sut.RegisterPlayer(p1);
            sut.RegisterPlayer(p2);

            sut.RecordMatch(p1, p2);

            Assert.Equal(startingElo, sut.CalculateElo(p1), 1);
            Assert.Equal(startingElo, sut.CalculateElo(p2), 1);

            sut.RecordMatch(p1, p2, MatchWinner.Draw);

            Assert.Equal(startingElo, sut.CalculateElo(p1), 1);
            Assert.Equal(startingElo, sut.CalculateElo(p2), 1);
        }

        [Theory]
        [InlineData(1500, 50)]
        [InlineData(500, 250)]
        public void RankingSystem_CalculateElo_WithCustomSettings(double startingElo, double kFactor)
        {
            var p1 = new Player();
            var p2 = new Player();
            var p3 = new Player();

            RankingSystem sut = new RankingSystem(startingElo, kFactor);
            sut.RegisterPlayer(p1);
            sut.RegisterPlayer(p2);
            sut.RegisterPlayer(p3);

            sut.RecordMatch(p1, p2, MatchWinner.PlayerA);

            Assert.Equal(startingElo + (kFactor / 2f), sut.CalculateElo(p1), 1);
            Assert.Equal(startingElo - (kFactor / 2f), sut.CalculateElo(p2), 1);
            Assert.Equal(startingElo, sut.CalculateElo(p3), 1);
        }

        [Fact]
        public void RankingSystem_CalculateElo_OverSeveralMatches()
        {
            var p1 = new Player();
            var p2 = new Player();
            var p3 = new Player();

            RankingSystem sut = new RankingSystem();
            sut.RegisterPlayer(p1);
            sut.RegisterPlayer(p2);
            sut.RegisterPlayer(p3);

            sut.RecordMatch(p1, p2, MatchWinner.PlayerA);
            sut.RecordMatch(p1, p2, MatchWinner.PlayerA);
            sut.RecordMatch(p1, p3, MatchWinner.Draw);
            sut.RecordMatch(p3, p2, MatchWinner.PlayerB);

            var actualP1 = sut.CalculateElo(p1);
            var actualP2 = sut.CalculateElo(p2);
            var actualP3 = sut.CalculateElo(p3);

            Assert.Equal(1118.0, actualP1, 1);
            Assert.Equal(1117.7, actualP2, 1);
            Assert.Equal(764.3, actualP3, 1);
        }
    }
}