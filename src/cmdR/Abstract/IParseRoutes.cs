using System.Collections.Generic;

namespace cmdR.Abstract
{
    public interface IParseRoutes
    {
        IDictionary<string, ParameterType> Parse(string route, out string commandName);
    }
}
