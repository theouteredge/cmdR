using cmdR.Abstract;
using cmdR.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.CommandParsing
{
    public class KeyValueCommandParser : CommandParserBase, IParseCommands
    {
        public void SetRoutes(List<Route> routes)
        {
            // do nothing, we dont care about the routes
        }


        public IDictionary<string, string> Parse(string command, out string commandName)
        {
            var result = new Dictionary<string, string>();
            var position = 0;
            var nextposition = 0;

            command = command.Trim();

            commandName = GetUnescappedToken(command, ' ', position, out nextposition);
            position = nextposition;

            while (position < command.Length)
            {
                var paramName = GetUnescappedToken(command, ' ', position, out nextposition);
                position = nextposition;

                // the escape for a grouping char of " is \"
                var paramValue = GetEscappedToken(command, ' ', '"', "\\\"", position, out nextposition);
                position = nextposition;
                
                // could we get both a param name and param value out of the command? if not quit out as we are not iterested in just a param name
                if (string.IsNullOrEmpty(paramName) && string.IsNullOrEmpty(paramValue))
                    break;

                result.Add(paramName, paramValue);
            }

            return result;
        }
    }
}
