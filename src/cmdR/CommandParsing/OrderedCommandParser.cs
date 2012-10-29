using cmdR.Abstract;
using cmdR.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace cmdR.CommandParsing
{
    public class OrderedCommandParser : CommandParserBase, IParseCommands
    {
        private List<IRoute> _routes;

        public void SetRoutes(List<IRoute> routes)
        {
            _routes = routes;
        }

        public IDictionary<string, string> Parse(string command, out string commandName)
        {
            if (_routes == null)
                throw new NoRoutesSetupException("The command cannot be parsed as no routes have been setup");

            var result = new Dictionary<string, string>();
            var paramValues = new List<string>();
            var position = 0;
            var nextposition = 0;

            command = command.Trim();

            var cmdName = GetUnescappedToken(command, ' ', position, out nextposition);
            position = nextposition;
            commandName = cmdName;

            // get all the param values from the command
            while (position < command.Length)
            {
                var value = GetEscappedToken(command, ' ', '"', "\\\"", position, out nextposition);
                position = nextposition;

                paramValues.Add(value);
            }

            var routes = _routes.Where(x => x.Name == cmdName).ToList();
            if (routes.Count > 0)
            {
                // we have a registered route with the same command name, lets try and match up the parameters
                foreach (var route in routes)
                {
                    var required = route.RequiredParametersCount();
                    var total = route.TotalParametersCount();

                    if (required <= paramValues.Count && paramValues.Count <= total)
                    {
                        var i = 0;
                        foreach (var name in route.GetParmaNames())
                        {
                            if (i + 1 <= paramValues.Count)
                                result.Add(name, paramValues[i++]);
                        }

                        return result;
                    }
                }
            }

            throw new InvalidCommandException("The command is invalid, we could not bind the parameters to any route which have been setup");
        }
    }
}
