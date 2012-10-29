using cmdR.Abstract;
using cmdR.CommandParsing;
using cmdR.Exceptions;
using cmdR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public class CmdR
    {
        private IParseCommands _commandParser;
        private IRouteCommands _commandRouter;
        private IParseRoutes _routeParser;

        private ICmdRState _state;
        private ICmdRConsole _console;


        public CmdR(string cmdPrompt = "> ", string[] exitcodes = null)
        {
            this.Init(new OrderedCommandParser(), new Routing(), new RouteParser(), new CmdRConsole(), exitcodes, cmdPrompt);
        }

        public CmdR(IParseCommands parser, IRouteCommands routing, IParseRoutes routeParser, ICmdRConsole console, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            this.Init(parser, routing, routeParser, console, exitcodes, cmdPrompt);
        }


        private void Init(IParseCommands parser, IRouteCommands routing, IParseRoutes routeParser, ICmdRConsole console, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            _state = new CmdRState();
            _state.CmdPrompt = cmdPrompt;

            if (exitcodes != null)
                _state.ExitCodes = exitcodes;

            _console = console;

            _commandParser = parser;
            _commandRouter = routing;
            _routeParser = routeParser;
            
            _state.Routes = routing.GetRoutes();

            this.RegisterRoute("?", ListAllTheCommands);
        }

        private void ListAllTheCommands(IDictionary<string, string> parameters, ICmdRConsole console, ICmdRState state)
        {
            foreach (var route in state.Routes)
            {
                console.Write("    {0}", route.Name);

                foreach (var p in route.GetParameters())
                {
                    if (p.Value == ParameterType.Required)
                        console.Write(" [{0}]", p.Key);
                    else
                        console.Write(" <{0}>", p.Key);
                }

                console.WriteLine("");
            }
        }



        public void RegisterRoute(string route, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action)
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

            _commandParser.SetRoutes(_commandRouter.GetRoutes());

            var commandName = "";
            var parameters = _commandParser.Parse(command, out commandName);
            var route = _commandRouter.FindRoute(commandName, parameters);

            route.Execute(parameters, _console, _state);
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
                    _console.WriteLine("An exception was thrown while running your command\n  Message: {0}\n  Trace: {1}", e.Message, e.StackTrace);
                }

                // todo: wrap both of these lines in interfaces so we can abstract away the underlying UI framework, so cmdR can live within a WPF or WinForm app.
                _console.Write(_state.CmdPrompt);
                command = Console.ReadLine();
            }
            while (!_state.ExitCodes.Contains(command));
        }
    }
}