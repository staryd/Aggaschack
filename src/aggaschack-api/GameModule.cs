using Nancy;

namespace Äggaschack.Api 
{
    public class GameModule : NancyModule
    {
        public GameModule() : base("/game") 
        {
            Get("/", _ => "Äggaschack API!");
        }
    }
}