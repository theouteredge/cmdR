using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cmdR.Abstract;
using cmdR.CommandParsing;
using cmdR.Exceptions;
using cmdR.IO;

namespace cmdR
{
    public class CmdR
    {
        private IParseCommands _commandParser;
        private IRouteCommands _commandRouter;
        private IParseRoutes _routeParser;

        private ICmdRState _state;
        private ICmdRConsole _console;


        public ICmdRState State { get { return _state; } }
        public ICmdRConsole Console { get { return _console; } }

        

        public CmdR(string cmdPrompt = "> ", string[] exitcodes = null)
        {
            this.Init(new OrderedCommandParser(), new Routing(), new RouteParser(), new CmdRConsole(), new CmdRState(), exitcodes, cmdPrompt);
        }

        public CmdR(IParseCommands parser = null, IRouteCommands routing = null, IParseRoutes routeParser = null, ICmdRConsole console = null, ICmdRState state = null, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            this.Init(parser ?? new OrderedCommandParser(), routing ?? new Routing(), routeParser ?? new RouteParser(), console ?? new CmdRConsole(), state ?? new CmdRState(), exitcodes, cmdPrompt);
        }


        private void Init(IParseCommands parser, IRouteCommands routing, IParseRoutes routeParser, ICmdRConsole console, ICmdRState state, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            _state = state;
            _state.CmdPrompt = cmdPrompt;
            _state.Routes = routing.GetRoutes();
            _state.ExitCodes = exitcodes ?? new string[] { "exit" };

            _console = console;

            _commandParser = parser;
            _commandRouter = routing;
            _routeParser = routeParser;
        }



        public void RegisterRoute(string route, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action, string description = null)
        {
            if (string.IsNullOrEmpty(route.Trim()))
                throw new InvalidRouteException(string.Format("An empty route is invalid", route));

            var name = "";
            var parameters = _routeParser.Parse(route, out name);
            
            _commandRouter.RegisterRoute(name, parameters, action, description);
        }


        public void RegisterRoute(string route, Action<IDictionary<string, string>, CmdR> action, string description = null)
        {
            if (string.IsNullOrEmpty(route.Trim()))
                throw new InvalidRouteException(string.Format("An empty route is invalid", route));

            var name = "";
            var parameters = _routeParser.Parse(route, out name);

            _commandRouter.RegisterRoute(name, parameters, (dictionary, console, state) => action.Invoke(dictionary, this), description);
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


        public void ExecuteCommands(List<string> commands)
        {
            foreach (var command in commands)
            {
                ExecuteCommand(command);
            }
        }


        public void Run(string[] args)
        {
            // Run the initial commands
            var commands = this.ConstructMultipleCommands(args);
            try
            {
                this.ExecuteCommands(commands);
            }
            catch (Exception e)
            {
                _console.WriteLine("An exception was thrown while running your initial commands\n  Message: {0}\n  Trace: {1}", e.Message, e.StackTrace);
            }

            // Allow more commands to be entered via UI
            var command = string.Empty;

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

                _console.Write(_state.CmdPrompt);
                command = _console.ReadLine();
            }
            while (!_state.ExitCodes.Contains(command));
        }
    }
}