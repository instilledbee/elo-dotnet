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

            var rankings = new RankingSystem();
            rankings.RegisterPlayer(p1);
            rankings.RegisterPlayer(p2);
            rankings.RegisterPlayer(p3);

            rankings.RecordMatch(p1, p2, MatchWinner.PlayerA);

            Assert.Equal(1200f, rankings.CalculateElo(p1), 1);
            Assert.Equal(800f, rankings.CalculateElo(p2), 1);
            Assert.Equal(1000f, rankings.CalculateElo(p3), 1);
        }

        [Theory]
        [InlineData(1500, 50)]
        [InlineData(500, 250)]
        public void RankingSystem_CalculateElo_WithCustomSettings(double startingElo, double kFactor)
        {
            var p1 = new Player();
            var p2 = new Player();
            var p3 = new Player();

            var rankings = new RankingSystem(startingElo, kFactor);
            rankings.RegisterPlayer(p1);
            rankings.RegisterPlayer(p2);
            rankings.RegisterPlayer(p3);

            rankings.RecordMatch(p1, p2, MatchWinner.PlayerA);

            Assert.Equal(startingElo + (kFactor / 2f), rankings.CalculateElo(p1), 1);
            Assert.Equal(startingElo - (kFactor / 2f), rankings.CalculateElo(p2), 1);
            Assert.Equal(startingElo, rankings.CalculateElo(p3), 1);
        }

        [Fact]
        public void RankingSystem_CalculateElo_OverSeveralMatches()
        {
            var p1 = new Player();
            var p2 = new Player();
            var p3 = new Player();

            var rankings = new RankingSystem();
            rankings.RegisterPlayer(p1);
            rankings.RegisterPlayer(p2);
            rankings.RegisterPlayer(p3);

            rankings.RecordMatch(p1, p2, MatchWinner.PlayerA);
            rankings.RecordMatch(p1, p2, MatchWinner.PlayerA);
            rankings.RecordMatch(p1, p3, MatchWinner.Draw);
            rankings.RecordMatch(p3, p2, MatchWinner.PlayerB);

            var actualP1 = rankings.CalculateElo(p1);
            var actualP2 = rankings.CalculateElo(p2);
            var actualP3 = rankings.CalculateElo(p3);

            Assert.Equal(1118.0, actualP1, 1);
            Assert.Equal(1117.7, actualP2, 1);
            Assert.Equal(764.3, actualP3, 1);
        }
    }
}