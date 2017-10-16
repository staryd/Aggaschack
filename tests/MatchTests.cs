using System.Linq;
using Äggaschack.Core;
using Xunit;

namespace Äggaschack.Tests
{
    public class MatchTests
    {
        Player playerOne;
        Player playerTwo;

        public MatchTests()
        {
            playerOne = new Player("one");
            playerTwo = new Player("two");
        }

        [Fact]
        public void Match_is_created_with_two_players()
        {
            var match = new Match(playerOne, playerTwo);

            Assert.Equal(playerOne, match.Players[0]);
            Assert.Equal(playerTwo, match.Players[1]);
        }

        [Fact]
        public void Match_randomly_decides_who_gets_to_play_first() 
        {
            var match = new Match(playerOne, playerTwo);
            match.RandomizerFunction = () => 0;

            Assert.Equal(playerOne, match.ActivePlayer);
        }

        [Fact]
        public void Match_generates_empty_board()
        {
            var match = new Match(playerOne, playerTwo);

            var board = match.Board;

            Assert.Equal(match.BoardSize * match.BoardSize, board.Count);
            foreach(var cell in board)
                Assert.Equal(SquareState.Empty, cell.State);
        }

        [Fact]
        public void Match_throws_exception_if_not_players_turn() 
        {
            var match = new Match(playerOne, playerTwo);
            match.RandomizerFunction = () => 0;

            Assert.Throws<NotYourTurnException>(() => match.Play(playerTwo, 0, 0, Egg.Full));
        }

        [Fact]
        public void Valid_move_switches_active_player() 
        {
            var match = new Match(playerOne, playerTwo);
            var activePlayer = match.ActivePlayer;

            match.Play(activePlayer, 0, 0, Egg.Full);
            Assert.NotEqual(activePlayer, match.ActivePlayer);
        }

        [Fact]
        public void Move_with_illegal_coordinate_throws_exception()
        {
            var match = new Match(playerOne, playerTwo);
            var activePlayer = match.ActivePlayer;

            Assert.Throws<IllegalMoveException>(() => match.Play(activePlayer, 4, 4, Egg.Full));
        }

        [Fact]
        public void Valid_move_updates_square_state()
        {
            var match = new Match(playerOne, playerTwo);
            var activePlayer = match.ActivePlayer;

            match.Play(activePlayer, 0, 0, Egg.Half);

            Assert.Equal(SquareState.HalfEgg, match.Board[0].State);
        }

        [Fact]
        public void Move_with_invalid_state_throws_exception() 
        {
            var board = Enumerable.Range(0, 9).Select(x => new Square(x == 7 ? SquareState.HalfEgg : SquareState.Empty)).ToList();
            var match = new Match(playerOne, playerTwo, board);
            var activePlayer = match.ActivePlayer;

            Assert.Throws<IllegalMoveException>(() => match.Play(activePlayer, 1, 2, Egg.Full));
            Assert.Equal(SquareState.HalfEgg, match.Board[7].State);
        }

        [Fact]
        public void Winning_move_rank_sets_matchstate_to_won() 
        {
            var board = Enumerable.Range(0, 9).Select(x => new Square(x < 2 ? SquareState.FullEgg : SquareState.Empty)).ToList();
            var match = new Match(playerOne, playerTwo, board);
            var player = match.ActivePlayer;

            match.Play(player, 2, 0, Egg.Full);

            Assert.True(match.Finished);
            Assert.Equal(player, match.Winner);
            Assert.Null(match.ActivePlayer);
        }
    }
}
