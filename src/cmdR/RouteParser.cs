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
            var routeparts = route == null ? new List<string>() : route.Split(' ').Where(x => !string.IsNullOrEmpty(x));

            if (routeparts.Count() == 0)
                throw new InvalidRouteException("the round does not have any parts, you have to supply at least a command name");

            commandName = routeparts.Take(1).Single();
            var param = ConvertToParameterDictionary(routeparts.Skip(1));

            this.ValidateParameterOrdering(param);

            return param;
        }

        private void ValidateParameterOrdering(IDictionary<string, ParameterType> param)
        {
            // ensure that there are no optional parameters before required parameters
            var lastRequired = LastRequiredParameter(param);
            var firstOptional = FirstOptionalParameter(param);

            if (firstOptional != -1 && firstOptional < lastRequired)
                throw new InvalidRouteException("Optional parameters are not allowed before required parameters");
        }

        private int LastRequiredParameter(IDictionary<string, ParameterType> param)
        {
            var i = 0;
            var index = -1;
            
            foreach (var p in param)
            {
                if (p.Value == ParameterType.Required)
                    index = i;

                i++;
            }

            return index;
        }

        private int FirstOptionalParameter(IDictionary<string, ParameterType> param)
        {
            var i = 0;
            
            foreach (var p in param)
            {
                if (p.Value == ParameterType.Optional)
                    return i;

                i++;
            }

            return -1;
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
}
