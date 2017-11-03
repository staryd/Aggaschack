using System;
using System.Linq;
using System.Threading.Tasks;
using Äggaschack.Api;
using Äggaschack.Core;
using Nancy;
using Nancy.Testing;
using Xunit;

namespace  Äggaschack.Tests.Api
{
    public class CreateOrJoinTests 
    {
        FakeGameRepository gameRepository;
        Browser browser;

        public CreateOrJoinTests()
        {
            gameRepository = new FakeGameRepository();
            var bootstrapper = new ConfigurableBootstrapper(with => {
                with.Dependency<IGameRepository>(gameRepository);
                with.Module<GameModule>();
            });
            browser = new Browser(bootstrapper);
        }

        private void WithDefaultValues(BrowserContext with) {
            with.HttpRequest();
            with.Header("PlayerToken", "playerone");
            with.Header("Accept", "application/json");
        }

        [Fact]
        public async Task FirstPlayer_Returns_Match_With_Status_WaitingForOtherPlayer()
        {
            gameRepository.TheId = "1";
            gameRepository.TheMatch = new NotYetStartedMatch(new Player("playerone"));

            var response = await browser.Post("/game/join", WithDefaultValues);
            var result = response.Body.DeserializeJson<GameInfo>();

            Assert.Equal("1", result.Id);
            Assert.Equal(GameStatus.WaitingForPlayerToJoin, result.Status);
        }

        [Theory]
        [InlineData(0, GameStatus.OtherPlayersTurn)]
        [InlineData(1, GameStatus.YourTurn)]
        public async Task SecondPlayer_Returns_StartedMatch_With_Correct_Status(int startPlayerIndex, GameStatus expectedStatus) 
        {
            gameRepository.TheId = "1";
            var match = new Match(new Player("playertwo"), new Player("playerone"));
            match.RandomizerFunction = () => startPlayerIndex;
            gameRepository.TheMatch = match;

            var response = await browser.Post("/game/join", WithDefaultValues);

            var result = response.Body.DeserializeJson<GameInfo>();

            Assert.Equal("1", result.Id);
            Assert.Equal("playertwo", result.OtherPlayer);
            Assert.Equal(expectedStatus, result.Status);
            Assert.NotNull(result.Board);
            Assert.True(result.Board.All(square => square.State == SquareState.Empty));
        }
    }

    public class FakeGameRepository : IGameRepository
    {
        public string TheId { get; set; }
        public Match TheMatch { get; set; }

        public (string id, Match match) CreateOrJoin(string user)
        {
            return (TheId, TheMatch);
        }
    }
}