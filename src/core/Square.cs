namespace Äggaschack.Core
{
    public enum SquareState { Empty, FullEgg, HalfEgg, ShellOnly }

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
}
