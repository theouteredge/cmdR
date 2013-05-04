cmdR
====
CmdR is a simple command routing framework for console applications, giving you a simple way of routing commands to an Action which will perform your apps logic. This helps introduce a separation between your apps logic and the code to drive the input-parse-execute loop.

CmdR works as a simple input-parse-execute loop where the user enters a command (input) which cmdR will parse and route (parse) to an Action for execution (execute) once the Action has completed cmdR will wait for the user to enter another command and repeat the process


NuGet
-----
Install-Package cmdR


COMMAND PARSING
---------------
CmdR comes with two different parsers Ordered (the default) and KeyValue.

The Ordered Parser uses the order of the parameters to link them with the commands parameter name. So for the command "cmd p1 p2" and input "cmd input1 input2" the parameters p1 will be linked to input1 an p2 to input2.

The KeyValue Parser requires that the parameters and commands are both input with the command. So for the command "cmd p1 p2" the input would need to be "cmd p1 input1 p2 input2", so the inputs are parsed in the format "parameter input".

To use the Orders parser you dont need to do anything as its the default. To use the KeyValue parser you need to specify it when constructing CmdR

    var cmdR = new CmdR(parser: new KeyValueCommandParser());


USAGE
-----
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

            // registering a route with a lambda and cmdR [v1.2.0]
            cmdR.RegisterRoute("echo2 text", (parameters, cmd) => 
                { 
                    cmd.Console.WriteLine(parameters["text"]);
                }));

            
            // start the cmdR loop passing in the args as the first command to execute
            cmdR.AutoRegisterCommands();
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
--------------

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



USAGE: Modules
--------------
    
    public class DirectoryModule : ICmdRModule
    {
        public DirectoryModule(CmdR cmdR)
        {
            cmdR.RegisterRoute("ls search?", List, "list all files and directories in the current path with an optional RegEx search pattern");
            cmdR.RegisterRoute("cd path", ChangeDirectory, "sets the currently active path, all subsequent commands will be executed within this path");
        }

        private void List(IDictionary<string, string> param, CmdR cmd)
        {
            //todo: list directories
        }

        private void ChangeDirectory(IDictionary<string, string> param, CmdR cmd)
        {
            if (Directory.Exists(param["path"]))
            {
                cmd.State.Variables["path"] = param["path"];
                cmd.State.CmdPrompt = string.Format("{0}\ncmdR>", param["path"]);
            }
            else cmd.Console.WriteLine("{0} does not exists", param["path"]);
        }
    }


USAGE: Single Command Class
---------------------------

    public class ChangeDirectoryCommand : ICmdRCommand
    {
        public string Command { get { return "cd path"; } }
        public string Description { get { return "sets the currently active path, all subsequent commands will be executed within this path"; } }
        
        public void Execute(IDictionary<string, string> param, CmdR cmd)
        {
            if (Directory.Exists(param["path"]))
            {
                cmd.State.Variables["path"] = param["path"];
                cmd.State.CmdPrompt = string.Format("{0}\ncmdR>", param["path"]);;
            }
            else cmd.Console.WriteLine("{0} does not exists", param["path"]);
        }
    }



VERSION HISTORY
---------------
__1.3.0__
Changed the ICmdRConsle so the Write and WriteLine methods are __params object[] paramters__ instead of __params string[] paramters__ no more .ToString() needed

Changed the ICmdRState so it has a __IDictionary<string, object> Variables { get; set; }__ this allows you to store variables and share them with other commands

Introduced two new Interfaces which allow you to easily register commands automatically, __ICmdRModule__ and __ICmdRCommand__

**ICmdRModule** allow you to register a single class which implements lots of commands. 
The CmdR class will be passed into the classes constructor allowing you to register your routes there.

**ICmdRCommand** allows you to register a single class which implements a single command.
This interface specifies a small interface which will be used by CmdR to automatically register your route.

Simply implement these interfaces and register them with CmdR by calling cmdR.AutoRegisterCommands()


__1.2.0__
Added an additional RegisterRoute which takes in the CmdR class itself. This reduces the amount of params you need to specify while giveing you the same functinality.

    Action<IDictionary<string, ParameterType>, ICmdR>

Cleaned up the help command so its not so damned ugly... still a little ugly
Cleaned the nuget spec aftr finding the -version switch, we don't need multiple nuspec's anymore


__1.1.0__
Two additonal parameters, ICmdRConsole and ICmdRState, have been added to the routes actions, so the action signitures have changed to:
    
    Action<IDictionary<string, ParameterType>, ICmdRConsole, ICmdRState>

__ICmdRConsole__ abstracts away the reliance on the built in Console class, so we can implement versions for other frameworks in the future, you should use this if you want  to output anything to the screen.

__ICmdRState__ gives you access to CmdR's internal state, allowing you to modify exit codes, the CmdPrompt and see the current collection of Registered Routes. This was mainly implemented to give you access to the CmdR CmdPrompt setting so you can modify it while the application is running to give feedback to the user. i.e. to show the current path or which database we are currently connected to.


FUTURE PLANS
------------
1. Startup message, so users can enter a message that will be displayed when cmdR.Run is called for the first time
2. Allow the setting of text colours when using the built in console.
3. Implement a way of pipeing commands together
   1. look at the way nodejs implements middlewhere with the next function
4. Implement a verison of the ICmdRConsole interface that wil work with a wpf app, so we can test out the ICmdRConsole interface in something other than a console app
5. Allow us to pump data from one command to annother, i.e. read file.txt |> count-words


LICENCE
-------
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.