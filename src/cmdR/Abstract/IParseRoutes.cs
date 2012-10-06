using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Abstract
{
    public interface IParseRoutes
    {
        IDictionary<string, ParameterType> Parse(string route, out string commandName);
    }
}
