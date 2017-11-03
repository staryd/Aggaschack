using System;
using System.Collections.Generic;
using System.Linq;
using Äggaschack.Core;

namespace Äggaschack.Api 
{
    public interface IGameRepository {
        (string id, Match match) CreateOrJoin(string user);
    }

    public class GameRepository : IGameRepository
    {
        private static readonly Dictionary<string, Match> ActiveMatches = new Dictionary<string, Match>();

        public (string id, Match match) CreateOrJoin(string user)
        {
            //OBS! Temporär, ej trådsäker kod, måste göras trådsäker och bättre!
            var match = ActiveMatches.FirstOrDefault(x => x.Value is NotYetStartedMatch);

            if (match.Key != null) {
                var newMatch = new Match(match.Value.Players[0], new Player(user));
                ActiveMatches[match.Key] = newMatch;
                return(match.Key, newMatch);
            }

            var m = new NotYetStartedMatch(new Player(user));
            var id = Guid.NewGuid().ToString();
            ActiveMatches[id] = m;
            return (id, m);
        }
    }
}