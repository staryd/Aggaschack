using System;

namespace Äggaschack.Core 
{

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
    
}
