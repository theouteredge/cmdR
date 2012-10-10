using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Abstract
{
    public interface IParseCommands
    {
        void SetRoutes(List<Route> routes);

        IDictionary<string, string> Parse(string command, out string commandName);
    }
}
