using cmdR.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR
{
    public class CmdR
    {
        private IParseCommands _commandParser;
        private IRouteCommands _commandRouter;

        private readonly string[] _exitcodes = new[] { "exit" };
        private readonly string _cmdPrompt;

        
        public CmdR(string[] exitcodes = null, string cmdPrompt = "  > ")
        {
            if (exitcodes != null)
                _exitcodes = exitcodes;

            _cmdPrompt = cmdPrompt;
        }



        public void RegisterRoute(string route, Action<IDictionary<string, string>> action)
        {
            if (string.IsNullOrEmpty(route))
                throw new InvalidRouteException(string.Format("The route is invalid", route));


            var routeparts = route.Split(' ').Where(x => !string.IsNullOrEmpty(x));

            var name = routeparts.Take(1).Single();
            var parameters = ConvertToParameterDictionary(routeparts.Skip(1));

            _commandRouter.RegisterRoute(name, parameters, action);
        }

        private IDictionary<string, bool> ConvertToParameterDictionary(IEnumerable<string> parameters)
        {
            var result = new Dictionary<string, bool>();
            foreach (var param in parameters)
            {
                // params with a question mark at the end are optional
                if (param.Last() == '?')
                    result.Add(param, false);
                else
                    result.Add(param.Substring(0, param.Length-1), true);
            }

            return result;
        }


        public void Run(string[] args)
        {
            var command = string.Join(" ", args);
            var commandName = "";

            do
            {
                try
                {
                    var parameters = _commandParser.Parse(command, out commandName);
                    var route = _commandRouter.FindRoute(commandName, parameters);

                    route.Execute(parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception was thrown while running your command");
                    Console.WriteLine("  Message: " + e.Message);
                    Console.WriteLine("  Trace: " + e.StackTrace);
                }

                Console.Write(_cmdPrompt);
                command = Console.ReadLine();
            }
            while (!_exitcodes.Contains(command));
        }
    }
}
