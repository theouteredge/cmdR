using cmdR.IO;
using System;
using System.Collections.Generic;

namespace cmdR.Abstract
{
    public interface IRouteCommands
    {
        void RegisterRoute(string commandName, IDictionary<string, ParameterType> parameters, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action, string description = null);
        void RegisterRoute(IRoute route);

        List<IRoute> GetRoutes();

        IRoute FindRoute(string commandName, IDictionary<string, string> parameters);
    }
}
