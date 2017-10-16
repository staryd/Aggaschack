using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace aggaschack
{
    public class BoardCheckerTests 
    {
        [Theory]
        [InlineData("333000000")]
        [InlineData("123222000")]
        [InlineData("000000111")]
        public void Rank_with_same_state_is_winner(string boardState)
        {
            var board = CreateBoard(boardState);
            var boardChecker = new BoardChecker(3);

            Assert.True(boardChecker.CheckRanks(board));
        }
        
        private IEnumerable<Square> CreateBoard(string boardState) {
            foreach(var c in boardState) {
                switch(c.ToString()) {
                    case "0": yield return new Square(); break;
                    case "1": yield return new Square(SquareState.ShellOnly); break;
                    case "2": yield return new Square(SquareState.HalfEgg); break;
                    case "3": yield return new Square(SquareState.FullEgg); break;
                }       
            }
        }
    }

    public class BoardChecker {

        private readonly int boardSize;

        public BoardChecker(int boardSize) {
            this.boardSize = boardSize;
        }

        public bool CheckRanks(IEnumerable<Square> board)
        {
            var squareList = board.ToList();
            for(var rank = 0; rank < boardSize; rank++)
            {
                var state = squareList[rank].State;

                if (state == SquareState.Empty)
                    continue;

                var done = false;

                for(var file = 1; file < boardSize; file++) {
                    if (squareList[rank*boardSize + file].State != state) {
                        done = true;
                        break;
                    }
                }    

                if (done)
                    continue;

                return true;
            }

            return false;
        }
    }
}