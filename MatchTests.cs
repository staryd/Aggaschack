using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace aggaschack
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

    public class AggaschackException : Exception {
        public AggaschackException() : base() {}
        public AggaschackException(string message) : base(message) {}
    }

    public class NotYourTurnException : AggaschackException {
        public NotYourTurnException() : base() {}
        public NotYourTurnException(string message) : base(message) {}
    }
    public class IllegalMoveException : AggaschackException {
        public IllegalMoveException() : base() {}
        public IllegalMoveException(string message) : base(message) {}
    }    

    public class Match
    {
        private int activePlayerIndex;
        public readonly int BoardSize = 3;

        public Match(Player playerOne, Player playerTwo)
        {
            Players = new List<Player> { playerOne, playerTwo };
            activePlayerIndex = RandomizerFunction();
            
            Board = Enumerable.Range(0, BoardSize * BoardSize).Select(i => new Square()).ToList();
        }

        internal Match(Player playerOne, Player playerTwo, List<Square> board) : this(playerOne, playerTwo)
        {
            Board = board;
        }

        public IReadOnlyList<Player> Players { get; } 

        internal Func<int> RandomizerFunction { get; set; } = () => new Random().Next(1);

        public Player ActivePlayer => Finished ? null : Players[activePlayerIndex];

        public IReadOnlyList<Square> Board { get; }

        public Player Winner { get; private set; }
        public bool Finished { get; private set; }

        

        public void Play(Player player, int xCoord, int yCoord, Egg move)
        {
            if (player != ActivePlayer)
                throw new NotYourTurnException();

            CheckValidMove(xCoord, yCoord, move);

            var square = Board[yCoord * BoardSize + xCoord];
            square.UpdateState(move);

            //Check ranks
            var boardChecker = new BoardChecker(BoardSize);
            var won = boardChecker.CheckRanks(Board);
            if (won) {
                Finished = true;
                Winner = player;
            }
            
            activePlayerIndex = activePlayerIndex == 1 ? 0 : 1;
        }

        private void CheckValidMove(int xCoord, int yCoord, Egg move) {
            if (new [] { xCoord, yCoord }.Any(coord => coord < 0 || BoardSize <= coord))
                throw new IllegalMoveException($"{xCoord},{yCoord} is not on the board!");
        }
    }

    public class Player
    {
        private string name;

        public Player(string name)
        {
            this.name = name;
        }
    }

    public class Square 
    {
        public SquareState State { get; private set; } 
        public Square(SquareState state = SquareState.Empty) 
        {
            State = state;
        }

        internal void UpdateState(Egg move)
        {
            SquareState? newState = null;
            switch(move){
                case Egg.Full: newState = SquareState.FullEgg; break;
                case Egg.Half: newState = SquareState.HalfEgg; break;
                case Egg.Empty: newState = SquareState.ShellOnly; break;
            }

            if (newState < State)
                throw new IllegalMoveException();

            State = newState.Value;
        }
    }

    public enum SquareState { Empty, FullEgg, HalfEgg, ShellOnly }
    public enum Egg { Full, Half, Empty }
}
