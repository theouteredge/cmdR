using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.UI.Dos
{
    class Program
    {
        static void Main(string[] args)
        {
            // the class which contains all our logic
            var example = new DOSPromptReplication();

            // creating the CmdR class passing, specifying the command prompt (> ) to use and a list of exit codes (exit) the user can type to exit the cmdR loop
            // these are the system defaults, so they dont actually need to be passed in
            var cmdR = new CmdR("> ", new string[] { "exit" });
            
            // setting up the command routes
            cmdR.RegisterRoute("cd path", example.ChangeDirectory, "change current directory");
            cmdR.RegisterRoute("ls filter?", example.ListDirectory, "list the contents of the current directory, optionaly specify a filter to search for contents within the directory");
            cmdR.RegisterRoute("del file", example.DeleteFile, "delete the sepcified file");

            // registering a route with a lambda
            cmdR.RegisterRoute("echo text", (parameters, console, state) => console.WriteLine(parameters["text"]), "print the specified text to the console");

            
            // start the cmdR loop passing in the args as the first command to execute
            cmdR.Run(args);
        }
    }
}
