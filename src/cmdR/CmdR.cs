﻿using cmdR.Abstract;
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
            this.Init(new OrderedCommandParser(), new Routing(), new RouteParser(), new CmdRConsole(), new CmdRState(), exitcodes, cmdPrompt);
        }

        public CmdR(IParseCommands parser, IRouteCommands routing, IParseRoutes routeParser, ICmdRConsole console, ICmdRState state, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            this.Init(parser, routing, routeParser, console, state, exitcodes, cmdPrompt);
        }


        private void Init(IParseCommands parser, IRouteCommands routing, IParseRoutes routeParser, ICmdRConsole console, ICmdRState state, string[] exitcodes = null, string cmdPrompt = "> ")
        {
            _state = state;
            _state.CmdPrompt = cmdPrompt;
            _state.Routes = routing.GetRoutes();

            if (exitcodes != null)
                _state.ExitCodes = exitcodes;
            else 
                _state.ExitCodes = new string[] { "exit" };

            _console = console;

            _commandParser = parser;
            _commandRouter = routing;
            _routeParser = routeParser;

            this.RegisterRoute("help", ListAllTheCommands, "lists all the commands");
        }



        public void RegisterRoute(string route, Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> action, string description = null)
        {
            if (string.IsNullOrEmpty(route.Trim()))
                throw new InvalidRouteException(string.Format("An empty route is invalid", route));

            var name = "";
            var parameters = _routeParser.Parse(route, out name);
            
            _commandRouter.RegisterRoute(name, parameters, action, description);
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
                command = _console.ReadLine();
            }
            while (!_state.ExitCodes.Contains(command));
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

                if (!string.IsNullOrEmpty(route.Description))
                    console.WriteLine("\n    {0}\n", route.Description);
                else 
                    console.WriteLine("\n");
            }
        }
    }
}