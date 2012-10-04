using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public interface IRouteCommands
    {
        Route FindRoute(string commandName, IDictionary<string, string> command);
    }
}
