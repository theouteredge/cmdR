using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public class Route
    {
        public string Name { get; set; }
        public IDictionary<string, bool> ParametersToTake { get; set; }
        public Action<IDictionary<string, string>> Action { get; set; }

        public Route(IDictionary<string, bool> parameterToTake, Action<IDictionary<string, string>> invoke)
        {
            ParametersToTake = parameterToTake;
            Action = invoke;
        }

        public bool Match(List<string> paramNames)
        {
            // does the amount of required params meet the params we where expecting?
            if (paramNames.Count != this.ParametersToTake.Where(x => x.Value).Count())
                return false;

            // check to see if we where expecing all the params which where passed in
            foreach(var param in paramNames)
            {
                if (!this.ParametersToTake.ContainsKey(param))
                    return false;
            }

            // check to see if all the required params where passed in
            foreach (var requiredParam in ParametersToTake.Where(x => x.Value).Select(x => x.Key))
            {
                if (!paramNames.Contains(requiredParam))
                    return false;
            }

            return true;
        }

        public void Execute(IDictionary<string, string> parameters)
        {
            Action.Invoke(parameters);
        }
    }
}
