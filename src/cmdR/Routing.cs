using cmdR.Abstract;
using cmdR.Exceptions;
using cmdR.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdR
{
    public class Routing : IRouteCommands
    {
        private List<IRoute> _routes = new List<IRoute>();


        public int Count { get { return _routes.Count; } }


        public void RegisterRoute(string name, IDictionary<string, ParameterType> parameters, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action, string description = null)
        {
            RegisterRoute(new Route(name, parameters, action, description));
        }

        public void RegisterRoute(IRoute route)
        {
            var matchingRoutes = _routes.Where(x => x.Name == route.Name).ToList();

            if (matchingRoutes.Count > 0)
            {
                foreach (var r in matchingRoutes)
                {
                    if (route.Match(r.GetParmaNames()))
                        throw new InvalidRouteException(string.Format("There is already a route registered which matches you route name [{0}] and parameters you supplied", route.Name));
                }
            }


            _routes.Add(route);
        }


        public List<IRoute> GetRoutes()
        {
            return _routes;
        }


        public IRoute FindRoute(string commandName, IDictionary<string, string> parameters)
        {
            var paramName = parameters.Select(x => x.Key).ToList();
            var routes = _routes.Where(x => x.Name == commandName && x.Match(paramName)).ToList();

            if (routes.Count == 0)
                throw new NoRouteFoundException(string.Format("No routes where found which match the parameter list: {0}", string.Join(",", paramName.ToArray())));

            if (routes.Count > 1)
                throw new ToManyRoutesFoundException(string.Format("{0} routes where found which matched the parameter list {2}, matched routes {1}", routes.Count, string.Join(",", routes.Select(x => x.Name).ToArray())));

            return routes[0];
        }
    }
}
