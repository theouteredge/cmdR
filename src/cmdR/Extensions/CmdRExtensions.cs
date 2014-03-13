using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using cmdR.Exceptions;

namespace cmdR
{
    public static class CmdRExtensions
    {
        public static string COMMAND_SEPARATOR = "&";
        public static string ESCAPE_CHAR = "\\";



        /// <summary>
        /// Constructs a list of commands using the COMMAND_SEPARATOR to split commands
        /// </summary>
        /// <param name="args">The parts of the commands</param>
        /// <returns>A list of commands</returns>
        public static List<string> ConstructMultipleCommands(this CmdR cmdR, string[] args)
        {
            var commands = new List<string>();

            if (args.Any() && !args.All(string.IsNullOrWhiteSpace))
            {
                var escapedArgs = EscapeEscapeChar(args);

                var i = 0;
                while (i < args.Count())
                {
                    var commandParts = escapedArgs.Skip(i).TakeWhile(arg => arg != COMMAND_SEPARATOR);
                    commands.Add(string.Join(" ", EscapeKeywords(commandParts)));

                    i = commands.Sum(x => x.Split(' ').Length) + commands.Count(); // sum of command parts and separators so far
                }
            }

            return commands;
        }


        private static IEnumerable<string> EscapeEscapeChar(IEnumerable<string> args)
        {
            return args.Select(arg =>
                {
                    if (arg == null)
                        return null;

                    return arg.Replace(ESCAPE_CHAR + ESCAPE_CHAR, ESCAPE_CHAR);
                });
        }

        private static IEnumerable<string> EscapeKeywords(IEnumerable<string> args)
        {
            return args.Select(arg =>
            {
                if (arg == null)
                    return null;

                return arg.Replace(ESCAPE_CHAR + COMMAND_SEPARATOR, COMMAND_SEPARATOR);
            });
        }



        /// <summary>
        /// Uses Reflection to loads and constructs all the ICmdRModules and ICmdRCommands from all the loaded assemblies and initalises them.
        /// This will also Register all Routes using CmdRouteAttributes
        /// </summary>
        public static void AutoRegisterCommands(this CmdR cmdR)
        {
            RegisterCommandModules(cmdR);

            var commands = FindAllTypesImplementingICmdRCommand();
            RegisterSingleCommands(cmdR, commands);
        }


        private static void RegisterCommandModules(CmdR cmdR)
        {
            var instances = GetICmdRModuleClasses().Select(c => Activator.CreateInstance(c, cmdR))
                                                   .ToArray();

            foreach (var module in instances)
            {
                var methods = module.GetType()
                                    .GetMethods()
                                    .Where(m => m.GetCustomAttributes(typeof(CmdRouteAttribute), false).Length > 0)
                                    .ToArray();

                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttributes(false)
                                          .SingleOrDefault(att => att is CmdRouteAttribute);

                    if (attribute != null)
                    {
                        var cmdRoute = (CmdRouteAttribute)attribute;

                        // todo: should we validate the type of parameter before trying to invoke the methods?
                        if (method.GetParameters().Count() == 1)
                        {
                            var meth = method;  // capture closure variables
                            var mod = module;

                            cmdR.RegisterRoute(cmdRoute.Route, param => meth.Invoke(mod, new object[] { param }), cmdRoute.Description);
                        }

                        else if (method.GetParameters().Count() == 2)
                        {
                            var meth = method;  // capture closure variables
                            var mod = module;

                            cmdR.RegisterRoute(cmdRoute.Route, (param, cmd) => meth.Invoke(mod, new object[] { param, cmd }), cmdRoute.Description);
                        }

                        else if (method.GetParameters().Count() == 3)
                        {
                            var meth = method;  // capture closure variables
                            var mod = module;

                            cmdR.RegisterRoute(cmdRoute.Route, (param, console, state) => meth.Invoke(mod, new object[] { param, console, state }), cmdRoute.Description);
                        }
                        else throw new InvalidMethodSignatureException("The method {0} with a [CmdRouteAttribute] does not have a valid signiture, expecting (Dictonary<string, object>, ICmdR) or (Dictonary<string, object>, ICmdRConsole, ICmdRState)");
                    }
                }
            }
        }


        private static IEnumerable<Type> GetICmdRModuleClasses()
        {
            var type = typeof(ICmdRModule);
            return AppDomain.CurrentDomain
                            .GetAssemblies()
                            .ToList()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);
        }

        private static void RegisterSingleCommands(CmdR cmdR, IEnumerable<ICmdRCommand> commands)
        {
            foreach (var cmd in commands)
                cmdR.RegisterRoute(cmd.Command, cmd.Execute, cmd.Description);
        }

        private static IEnumerable<ICmdRCommand> FindAllTypesImplementingICmdRCommand()
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
