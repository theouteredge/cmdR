using cmdR.Abstract;
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
        private List<Route> _routes = new List<Route>();


        public void RegisterRoute(string name, IDictionary<string, ParameterType> parameters, Action<IDictionary<string, string>> action)
        {
            _routes.Add(new Route(name, parameters, action));
        }

        public void RigisterRoute(Route route)
        {
            _routes.Add(route);
        }


        public Route FindRoute(string commandName, IDictionary<string, string> parameters)
        {
            var paramName = parameters.Select(x => x.Key).ToList();
            var routes = _routes.Where(x => x.Name == commandName && x.Match(paramName)).ToList();

            if (routes.Count == 0)
                throw new NoRouteFoundException(string.Format("No routes where found which match the parameter list: {0}", string.Join(",", paramName)));

            if (routes.Count > 1)
                throw new ToManyRoutesFoundException(string.Format("{0} routes where found which matched the parameter list {2}, matched routes {1}", routes.Count, string.Join(",", routes.Select(x => x.Name))));

            return routes[0];
        }
    }
}
