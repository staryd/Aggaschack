using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Äggaschack.Core;
using Nancy;

namespace Äggaschack.Api 
{
    public class GameModule : NancyModule
    {
        private readonly IGameRepository gameRepository;

        public GameModule(IGameRepository gameRepository) : base("/game") 
        {
            Get("/", _ => "Äggaschack API!");
            Post("/join", CreateOrJoin);
            this.gameRepository = gameRepository;
        }

        public async Task<object> CreateOrJoin(dynamic arg)
        {
            var (id, match) = gameRepository.CreateOrJoin(this.CurrentPlayer());

            if (match is NotYetStartedMatch) {
                return new GameInfo { Id = id, Status = GameStatus.WaitingForPlayerToJoin };
            }

            return new GameInfo {
                Id = id,
                OtherPlayer = match.Players.Single(x => x.Name != this.CurrentPlayer()).Name,
                Status = match.ActivePlayer.Name == this.CurrentPlayer() ? GameStatus.YourTurn : GameStatus.OtherPlayersTurn,
                Board = match.Board.Select(square => new SquareInfo { State = square.State }).ToArray()
            };
        }
    }

    public static class ModuleExtensions {
        public static string CurrentPlayer(this NancyModule module) {
            return module.Request.Headers["PlayerToken"].FirstOrDefault();
        }
    }

    public class GameInfo {
        public string Id { get; set; }
        public string OtherPlayer { get; set; }
        public SquareInfo[] Board { get; set; }

        public GameStatus Status { get; set; }
    }

    public class SquareInfo {
        public SquareState State { get; set; }
        public Egg[] ValidMoves { get; set; }
    }

    public enum GameStatus {
        YourTurn,
        OtherPlayersTurn,
        WaitingForPlayerToJoin,
        YouWon,
        OtherPlayerWon
    }
}