using System;
using System.Linq;
using Äggaschack.Core;

namespace aggaschack_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** Välkommen till Äggaschack! ***\r\n\r\n");

            Console.Write("Namn på spelare 1: ");
            var playerOneName = Console.ReadLine();
            var playerOne = new Player(playerOneName);

            Console.Write("Namn på spelare 2: ");
            var playerTwoName = Console.ReadLine();
            var playerTwo = new Player(playerTwoName);

            Console.WriteLine($"Välkomna, {playerOneName} och {playerTwoName}!");

            var match = new Match(playerOne, playerTwo);
            while(!match.Finished) {
                RenderBoard(match);

                Console.WriteLine($"Ditt drag, {match.ActivePlayer.Name}.");

                Console.Write("Ange koordinat (x,y): ");
                var coordInput = Console.ReadLine();
                var xCoord = int.Parse(coordInput.Substring(0, 1));
                var yCoord = int.Parse(coordInput.Substring(2, 1));

                Console.Write("Ange drag (O,u eller _): ");
                
                try {
                    var move = ParseEgg(Console.ReadLine());
                    match.Play(match.ActivePlayer, xCoord, yCoord, move);
                }
                catch (Äggaschack.Core.AggaschackException ex) {
                    Console.WriteLine($"\r\nERROR: {ex.Message}\r\n");
                }
            }

            Console.WriteLine($"Bra jobbat, {match.Winner.Name}! Du vann matchen!");
        }

        private static Egg ParseEgg(string str)
        {
            switch(str) {
                case "O": return Egg.Full;
                case "u": return Egg.Half;
                case "_": return Egg.Empty;
            }

            throw new IllegalMoveException($"{str} är inte ett giltigt drag!!");
        }

        private static void RenderBoard(Match match)
        {
            var width = match.BoardSize*2+1;

            //En rad med streck)
            RenderHorizontalRuler(width);
            for(var rank=match.BoardSize-1; rank>=0; rank--) {
                RenderRank(match, rank);
                RenderHorizontalRuler(width);
            }

            Console.WriteLine();
        }

        private static void RenderHorizontalRuler(int size) {
            Enumerable.Range(0,size).ToList().ForEach(x => Console.Write("-"));
            Console.WriteLine();
        }

        private static void RenderRank(Match match, int rankIndex) {
            Console.Write("|");
            for(var file=0; file<match.BoardSize; file++) {
                Console.Write(RenderState(match.Board[rankIndex*match.BoardSize+file].State));
                Console.Write("|");
            }
            Console.WriteLine();
        }

        private static string RenderState(SquareState state) {
            switch(state) {
                case SquareState.Empty: return " ";
                case SquareState.ShellOnly: return "_";
                case SquareState.HalfEgg: return "u";
                case SquareState.FullEgg: return "O";
            }

            throw new InvalidOperationException("WTF!!");
        }
    }
}
