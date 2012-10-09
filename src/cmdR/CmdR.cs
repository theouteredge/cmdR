using cmdR.Abstract;
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
        private IParseRoutes _routeParser;

        private readonly string[] _exitcodes = new[] { "exit" };
        private readonly string _cmdPrompt;


        
        public CmdR(string[] exitcodes = null, string cmdPrompt = "> ")
        {
            if (exitcodes != null)
                _exitcodes = exitcodes;

            _cmdPrompt = cmdPrompt;
            _commandParser = new KeyValueCommandParser();
            _commandRouter = new Routing();
        }

        public CmdR(IParseCommands parser, IRouteCommands routing, IParseRoutes _routeParser, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            if (exitcodes != null)
                _exitcodes = exitcodes;

            _cmdPrompt = cmdPrompt;
            _commandParser = parser;
            _commandRouter = routing;
        }



        public void RegisterRoute(string route, Action<IDictionary<string, string>> action)
        {
            if (string.IsNullOrEmpty(route.Trim()))
                throw new InvalidRouteException(string.Format("An empty route is invalid", route));

            var name = "";
            var parameters = _routeParser.Parse(route, out name);

            _commandRouter.RegisterRoute(name, parameters, action);
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
