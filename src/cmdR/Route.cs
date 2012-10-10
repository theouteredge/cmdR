using cmdR.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public class Route : IRoute
    {
        public string Name { get; set; }

        private IDictionary<string, ParameterType> ParametersToTake { get; set; }
        private Action<IDictionary<string, string>> Action { get; set; }

        public Route(string name, IDictionary<string, ParameterType> parameters, Action<IDictionary<string, string>> action)
        {
            this.Name = name;
            this.ParametersToTake = parameters;
            this.Action = action;
        }

        public bool Match(List<string> paramNames)
        {
            // does the amount of required params meet the params we where expecting?
            if (paramNames.Count < this.ParametersToTake.Where(x => x.Value == ParameterType.Required).Count())
                return false;

            // check to see if we where expecing all the params which where passed in
            foreach(var param in paramNames)
            {
                if (!this.ParametersToTake.ContainsKey(param))
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

        public void Execute(IDictionary<string, string> parameters)
        {
            Action.Invoke(parameters);
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
            return ParametersToTake.Where(x => x.Value == ParameterType.Required)
                                   .Count();
        }

        public int TotalParametersCount()
        {
            return ParametersToTake.Count;
        }
    }
}
