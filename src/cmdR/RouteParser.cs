using cmdR.Abstract;
using cmdR.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR
{
    public class RouteParser : IParseRoutes
    {
        public IDictionary<string, ParameterType> Parse(string route, out string commandName)
        {
            var routeparts = route.Split(' ').Where(x => !string.IsNullOrEmpty(x));

            if (routeparts.Count() == 0)
                throw new InvalidRouteException("the round does not have any parts, you have to supply at least a command name");

            commandName = routeparts.Take(1).Single();
            return ConvertToParameterDictionary(routeparts.Skip(1));
        }

        private IDictionary<string, ParameterType> ConvertToParameterDictionary(IEnumerable<string> parameters)
        {
            var result = new Dictionary<string, ParameterType>();
            foreach (var param in parameters)
            {
                // params with a question mark at the end are optional
                if (param.Last() == '?')
                    result.Add(param.Substring(0, param.Length - 1), ParameterType.Optional);
                else
                    result.Add(param, ParameterType.Required);
            }

            return result;
        }
    }

    public enum ParameterType
    {
        Required,
        Optional
    }
}
