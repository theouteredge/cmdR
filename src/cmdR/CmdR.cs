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



        public CmdR(string cmdPrompt = "> ", string[] exitcodes = null)
        {
            if (exitcodes != null)
                _exitcodes = exitcodes;

            _cmdPrompt = cmdPrompt;
            _commandParser = new KeyValueCommandParser();
            _commandRouter = new Routing();
            _routeParser = new RouteParser();
        }

        public CmdR(IParseCommands parser, IRouteCommands routing, IParseRoutes routeParser, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            if (exitcodes != null)
                _exitcodes = exitcodes;

            _cmdPrompt = cmdPrompt;
            _commandParser = parser;
            _commandRouter = routing;
            _routeParser = routeParser;
        }



        public void RegisterRoute(string route, Action<IDictionary<string, string>> action)
        {
            if (string.IsNullOrEmpty(route.Trim()))
                throw new InvalidRouteException(string.Format("An empty route is invalid", route));

            var name = "";
            var parameters = _routeParser.Parse(route, out name);

            _commandRouter.RegisterRoute(name, parameters, action);
        }



        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            var commandName = "";
            var parameters = _commandParser.Parse(command, out commandName);
            var route = _commandRouter.FindRoute(commandName, parameters);

            route.Execute(parameters);
        }



        public void Run(string[] args)
        {
            var command = string.Join(" ", args);

            do
            {
                try
                {
                    this.ExecuteCommand(command);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception was thrown while running your command\n  Message: {0}\n  Trace: {1}", e.Message, e.StackTrace);
                }

                // todo: wrap both of these lines in interfaces so we can abstract away the underlying UI framework, so cmdR can live within a WPF or WinForm app.
                Console.Write(_cmdPrompt);
                command = Console.ReadLine();
            }
            while (!_exitcodes.Contains(command));
        }
    }
}