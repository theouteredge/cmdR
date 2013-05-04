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

            this.RegisterRoute("help route?", ListAllTheCommands, "lists all the commands, or details about any commands which start with the supplied name if specified");
            this.RegisterRoute("? route?", ListAllTheCommands, "lists all the commands, or details about any commands which start with the supplied name if specified");
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
            if (parameters.ContainsKey("route"))
            {
                if (state.Routes.Any(x => x.Name.StartsWith(parameters["route"])))
                {
                    var route = state.Routes.Single(x => x.Name == parameters["route"]);

                    console.Write("  {0}", route.Name);

                    foreach (var p in route.GetParameters())
                        console.Write(p.Value == ParameterType.Required ? " {0}" : " {0}?", p.Key);
                    
                    console.WriteLine("");
                    if (!string.IsNullOrEmpty(route.Description))
                        console.WriteLine("  " + route.Description);
                }
                else console.WriteLine("  unknown command name [{0}]", parameters["route"]);
            }
            else
            {
                foreach (var route in state.Routes)
                    console.Write("{0}", route.Name.PadRight(20));

                console.WriteLine("");
            }
        }

        public void AutoRegisterCommands()
        {
            RegisterCommandModules();

            var commands = FindAllTypesImplementingICmdRCommand();
            RegisterSingleCommands(commands);
        }

        private void RegisterCommandModules()
        {
            var type = typeof(ICmdRModule);
            AppDomain.CurrentDomain.GetAssemblies()
                                   .ToList()
                                   .SelectMany(s => s.GetTypes())
                                   .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                                   .Select(c => Activator.CreateInstance(c, this))
                                   .ToArray();
        }

        private void RegisterSingleCommands(IEnumerable<ICmdRCommand> commands)
        {
            foreach(var cmd in commands)
                RegisterRoute(cmd.Command, cmd.Execute, cmd.Description);
        }

        private IEnumerable<ICmdRCommand> FindAllTypesImplementingICmdRCommand()
        {
            var type = typeof(ICmdRCommand);
            return AppDomain.CurrentDomain.GetAssemblies()
                                          .ToList()
                                          .SelectMany(s => s.GetTypes())
                                          .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                                          .Select(c => (ICmdRCommand)Activator.CreateInstance(c));
        }
    }
}