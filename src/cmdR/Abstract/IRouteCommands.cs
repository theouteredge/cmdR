using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public interface IRouteCommands
    {
        void RegisterRoute(string commandName, IDictionary<string, bool> parameters, Action<IDictionary<string, string>> action);
        Route FindRoute(string commandName, IDictionary<string, string> parameters);
    }
}
