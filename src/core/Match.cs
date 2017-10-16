using System;
using System.Collections.Generic;
using System.Linq;

namespace Äggaschack.Core
{
    public enum Egg { Full, Half, Empty }

    public class Match
    {
        private int activePlayerIndex;
        public readonly int BoardSize = 3;
        private readonly BoardChecker boardChecker;

        public Match(Player playerOne, Player playerTwo)
        {
            Players = new List<Player> { playerOne, playerTwo };
            activePlayerIndex = RandomizerFunction();
            
            Board = Enumerable.Range(0, BoardSize * BoardSize).Select(i => new Square()).ToList();
            boardChecker = new BoardChecker(BoardSize);
        }

        internal Match(Player playerOne, Player playerTwo, List<Square> board) : this(playerOne, playerTwo)
        {
            Board = board;
        }

        public IReadOnlyList<Player> Players { get; } 

        private static readonly Random random = new Random();
        internal Func<int> RandomizerFunction { get; set; } = () => random.Next(2);

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

            var won = boardChecker.CheckBoard(Board);
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
}
