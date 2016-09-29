using System.Collections.Generic;

namespace cmdR.Abstract
{
    public interface IParseCommands
    {
        void SetRoutes(List<IRoute> routes);

        IDictionary<string, string> Parse(string command, out string commandName);
    }
}
