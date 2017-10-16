using System;
using System.Collections.Generic;
using System.Linq;

namespace Äggaschack.Core
{
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
