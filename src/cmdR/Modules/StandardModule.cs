using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Modules
{
    public class StandardModule : ICmdRModule
    {
        private readonly CmdR _cmdR;

        public StandardModule(CmdR cmdR)
        {
            _cmdR = cmdR;
        }


        [CmdRoute("? route?", "lists all the commands, or details about any commands which start with the supplied name if specified", false)]
        public void ListAllTheCommands(IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("route"))
            {
                if (_cmdR.State.Routes.Any(x => x.Name.StartsWith(parameters["route"])))
                {
                    var route = _cmdR.State.Routes.Single(x => x.Name == parameters["route"]);

                    _cmdR.Console.Write("  {0}", route.Name);

                    foreach (var p in route.GetParameters())
                        _cmdR.Console.Write(p.Value == ParameterType.Required ? " {0}" : " {0}?", p.Key);

                    if (!string.IsNullOrEmpty(route.Description))
                        _cmdR.Console.WriteLine("  " + route.Description);
                }
                else _cmdR.Console.WriteLine("  unknown command name [{0}]", parameters["route"]);
            }
            else
            {
                foreach (var route in _cmdR.State.Routes)
                    _cmdR.Console.Write("{0}", route.Name.PadRight(20));

                _cmdR.Console.WriteLine("");
            }
        }
    }
}
