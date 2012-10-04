using cmdR.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR
{
    public class Routing : IRouteCommands
    {
        private readonly IParseCommands _commandParser;
        
        public readonly List<Route> Routes { get; private set; }


        public Routing(IParseCommands commandParser)
        {
            this.Routes = new List<Route>();
            _commandParser = commandParser;
        }


        public void RigisterRoute(Route route)
        {
            this.Routes.Add(route);
        }


        private IDictionary<string, string> ParseCommand(string command, out string commandName)
        {
            return _commandParser.Parse(command, out commandName);
        }


        public Route FindRoute(string command)
        {
            var commandName = "";
            var parameters = ParseCommand(command, out commandName);

            var paramName = parameters.Select(x => x.Key).ToList();
            var routes = this.Routes.Where(x => x.Name == commandName && x.Match(paramName)).ToList();

            if (routes.Count == 0)
                throw new NoRoutesFound(string.Format("No routes where found which match the parameter list: {0}", string.Join(",", paramName)));

            if (routes.Count > 1)
                throw new ToManyRoutesFound(string.Format("{0} routes where found which matched the parameter list {2}, matched routes {1}", routes.Count, string.Join(",", routes.Select(x => x.Name))));

            return routes[0];
        }
    }
}
