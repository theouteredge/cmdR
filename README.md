cmdR
===

CmdR is a simple command routing framework for console applications, giving you a simple way of routing commands 
to an Action which will perform your apps logic. This helps introduce a separation between your apps logic and the 
code to drive the input-parse-execute loop.

CmdR works as a simple input-parse-execute loop where the user enters a command (input) which cmdR will parse and 
route (parse) to an Action for execution (execute) once the Action has completed cmdR will wait for the user to 
enter another command and repeat the process


NuGet
=====

Install-Package cmdR


v1.1 Breaking Chnages
===

In this version we have implemented a small breaking change to the way a routes Action works, its now passed two additonal parameters ICmdRConsole and ICmdRState.

''ICmdRConsole'' abstracts away the reliance on the built in Console class, so we can implement versions for other frameworks in the future, you should use this if you want  to output anything to the screen.

''ICmdRState'' gives you access to CmdR's internal state, allowing you to modify exit codes, the CmdPrompt and see the current collection of Registered Routes. This was mainly implemented to give you access to the CmdR CmdPrompt setting so you can modify it while the application is running to give feedback to the user. i.e. to show the current path or which database we are currently connected to. 


Usage
=====

    class Program
    {
        static void Main(string[] args)
        {
            // the class which contains all our logic
            var example = new DOSPromptReplication();

            // creating the CmdR class passing, specifying the command prompt (> ) to use and a list of exit codes (exit) the user can type to exit the cmdR loop
            // these are the system defaults, so they dont actually need to be passed in
            var cmdR = new CmdR("c:\> ", new string[] { "exit" });
            
            // setting up the command routes
            cmdR.RegisterRoute("cd path", example.ChangeDirectory);
            cmdR.RegisterRoute("del file", example.DeleteFile);

            // registering a route with an optional parameter, optional params are denoted by the ? at the end
            cmdR.RegisterRoute("ls filter?", example.ListDirectory);

            // registering a route with a lambda
            cmdR.RegisterRoute("echo text", (parameters, console, state) => 
                { 
                    console.WriteLine(parameters["text"]);
                }));

            
            // start the cmdR loop passing in the args as the first command to execute
            cmdR.Run(args);
        }
    }
    
    public class DOSPromptReplication
    {
        private string _directory = @"c:\";

        public void ChangeDirectory(IDictionary<string, string> param, ICmdRConsole console, ICmdRState state)
        {
            var path = param["path"];

            if (Directory.Exists(path))
            {
                _directory = path;
            }
            else if (Directory.Exists(_directory + path))
            {
                _directory = _directory + path;
            }
            else console.WriteLine("{0} is not a valid directory", path);

            if (_directory.Last() != '\\')
                _directory = _directory + "\\";

            state.CmdPrompt = string.Format("{0}\n> ", _directory);
        }

        public void ListDirectory(IDictionary<string, string> param, ICmdRConsole console, ICmdRState state)
        {
            foreach(var file in Directory.GetFiles(_directory))
            {
                console.WriteLine(Path.GetFileName(file));
            }

            foreach (var directory in Directory.GetDirectories(_directory))
            {
                console.WriteLine(directory);
            }
        }

        public void DeleteFile(IDictionary<string, string> param, ICmdRConsole console, ICmdRState state)
        {
            var file = param["file"];

            if (File.Exists(file))
            {
                File.Delete(file);
            }
            else if (File.Exists(_directory + file))
            {
                File.Delete(_directory + file);
            }
            else console.WriteLine("{0} does not exist", file);
        }
    }


Example Output
=====

    c:\> echo "hello world!"
    hello world!
    c:\> cd c:\test
    c:\test> ls
    file1.txt
    file2.txt
    file3.txt
    c:\test> del file1.txt
    c:\test> ls
    file2.txt
    file3.txt
    c:\test> exit


Future Plans (maybe)
===

1. Startup message, so users can enter a message that will be displayed when cmdR.Run is called for the first time
2. Implement a verison of the ICmdRConsole interface that wil work with a wpf app, so we can test out the ICmdRConsole interface in something other than a console app
3. Maybe move to an MVC type framework which will allow you to return results which could forward you onto other routes allowing you to chain commands
4. Allow us to pump data from one command to annother, i.e. read file.txt |> count-words