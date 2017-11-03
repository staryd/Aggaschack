using System.Collections.Generic;
using Äggaschack.Core;

namespace Äggaschack.Api 
{
    public class NotYetStartedMatch : Match
    {
        public NotYetStartedMatch(Player playerOne) : base(playerOne, null, new List<Square>())
        {

        }
    }
}