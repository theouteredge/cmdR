using cmdR.Abstract;
using cmdR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public class Route : IRoute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        private IDictionary<string, ParameterType> ParametersToTake { get; set; }
        private Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> Action { get; set; }

        public Route(string name, IDictionary<string, ParameterType> parameters, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action, string description = null)
        {
            Name = name;
            ParametersToTake = parameters;
            Action = action;
            Description = description;
        }

        public bool Match(List<string> paramNames)
        {
            // does the amount of required params meet the params we where expecting?
            if (paramNames.Count < ParametersToTake.Count(x => x.Value == ParameterType.Required))
                return false;

            // check to see if we where expecing all the params which where passed in
            foreach(var param in paramNames.Where(x => !x.StartsWith("/")))
            {
                if (!ParametersToTake.ContainsKey(param))
                    return false;
            }

            // check to see if all the required params where passed in
            foreach (var requiredParam in ParametersToTake.Where(x => x.Value == ParameterType.Required).Select(x => x.Key))
            {
                if (!paramNames.Contains(requiredParam))
                    return false;
            }

            return true;
        }

        public void Execute(IDictionary<string, string> parameters, ICmdRConsole console, ICmdRState state)
        {
            Action.Invoke(parameters, console, state);
        }

        public List<string> GetParmaNames()
        {
            return ParametersToTake.Select(x => x.Key).ToList();
        }

        public IDictionary<string, ParameterType> GetParameters()
        {
            return ParametersToTake;
        }

        public int RequiredParametersCount()
        {
            return ParametersToTake.Count(x => x.Value == ParameterType.Required);
        }

        public int TotalParametersCount()
        {
            return ParametersToTake.Count;
        }
    }
}
