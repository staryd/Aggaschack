using System;
using System.Collections.Generic;
using System.Linq;
using Äggaschack.Core;
using Xunit;

namespace Äggaschack.Tests
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
}