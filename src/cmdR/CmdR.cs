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
