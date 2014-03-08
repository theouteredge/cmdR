using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.Modules
{
    public class StandardModule : ICmdRModule
    {
        [CmdRoute("? route?", "lists all the commands, or details about any commands which start with the supplied name if specified", false)]
        public void ListAllTheCommands(IDictionary<string, string> parameters, CmdR cmdR)
        {
            if (parameters.ContainsKey("route"))
            {
                if (cmdR.State.Routes.Any(x => x.Name.StartsWith(parameters["route"])))
                {
                    var route = cmdR.State.Routes.Single(x => x.Name == parameters["route"]);

                    cmdR.Console.Write("  {0}", route.Name);

                    foreach (var p in route.GetParameters())
                        cmdR.Console.Write(p.Value == ParameterType.Required ? " {0}" : " {0}?", p.Key);

                    cmdR.Console.WriteLine("");
                    if (!string.IsNullOrEmpty(route.Description))
                        cmdR.Console.WriteLine("  " + route.Description);
                }
                else cmdR.Console.WriteLine("  unknown command name [{0}]", parameters["route"]);
            }
            else
            {
                foreach (var route in cmdR.State.Routes)
                    cmdR.Console.Write("{0}", route.Name.PadRight(20));

                cmdR.Console.WriteLine("");
            }
        }
    }
}
