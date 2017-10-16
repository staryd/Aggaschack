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

        [Theory]
        [InlineData("200211233")] //File 0
        [InlineData("010010010")] //File 1
        [InlineData("203103103")] //File 2
        public void File_with_same_state_is_winner(string boardState)
        {
            var board = CreateBoard(boardState);
            var boardChecker = new BoardChecker(3);

            Assert.True(boardChecker.CheckFiles(board));
        }

        [Theory]
        [InlineData("100010001")]
        [InlineData("002020200")]
        public void Diagonal_with_same_state_is_winner(string boardState)
        {
            var board = CreateBoard(boardState);
            var boardChecker = new BoardChecker(3);

            Assert.True(boardChecker.CheckDiagonals(board));
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

        public bool CheckBoard(IEnumerable<Square> board) => CheckRanks(board) || CheckFiles(board) || CheckDiagonals(board);

        public bool CheckRanks(IEnumerable<Square> board)
        {
            var squareList = board.ToList();
            for(var rank = 0; rank < boardSize; rank++)
            {
                var state = squareList[rank*boardSize].State;

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

        public bool CheckFiles(IEnumerable<Square> board) 
        {
            var squareList = board.ToList();
            for(var file = 0; file < boardSize; file++)
            {
                var state =  squareList[file].State;

                if (state == SquareState.Empty)
                    continue;
                var done = false;

                for (var rank = 1; rank < boardSize; rank++) 
                {
                    if(squareList[rank*boardSize + file].State != state) {
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

        public bool CheckDiagonals(IEnumerable<Square> board) 
        {
            var squareList = board.ToList();

            var leftToRight = CheckDiagonal(squareList, 0, boardSize+1, squareList.Count);
            var rightToLeft = CheckDiagonal(squareList, boardSize-1, boardSize-1, squareList.Count-(boardSize-1));

            return leftToRight || rightToLeft;
        }

        private bool CheckDiagonal(List<Square> squareList, int startIndex, int step, int endBefore) 
        {
            var state = squareList[startIndex].State;
            if (state != SquareState.Empty) {
                bool allSame = true;

                for(var idx = startIndex+step; idx < endBefore; idx += step ) {
                    if (squareList[idx].State != state) {
                        allSame = false;
                        break;
                    }
                }

                if (allSame)
                    return true;
            }

            return false;
        }
    }
}