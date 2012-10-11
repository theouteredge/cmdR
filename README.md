cmdR
===

CmdR is a simple command routing framework for console applications, giving you a simple way of routing commands 
to an Action to perform your apps logic.

CmdR works as a simple loop where the user enters a command which cmdR will route to an Action for execution 
once the Action has completed cmdR will wait for the user to enter another command and repeat the process


NuGet
=====

Install-Package cmdR


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
            var cmdR = new CmdR("> ", new string[] { "exit" });
            
            // setting up the command routes
            cmdR.RegisterRoute("cd path", example.ChangeDirectory);
            cmdR.RegisterRoute("del file", example.DeleteFile);

            // registering a route with an optional parameter, optional params are denoted by the ? at the end
            cmdR.RegisterRoute("ls filter?", example.ListDirectory);

            // registering a route with a lambda
            cmdR.RegisterRoute("echo text", (parameters) => 
                { 
                    Console.WriteLine(parameters["text"]);
                }));

            
            // start the cmdR loop passing in the args as the first command to execute
            cmdR.Run(args);
        }
    }
    
    public class DOSPromptReplication
    {
        private string _directory = @"c:\";

        public void ChangeDirectory(IDictionary<string, string> param)
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
            else Console.WriteLine("{0} is not a valid directory", path);

            if (_directory.Last() != '\\')
                _directory = _directory + "\\";

            Console.WriteLine(_directory);
        }
        
        public void ListDirectory(IDictionary<string, string> param)
        {
            foreach(var file in Directory.GetFiles(_directory))
            {
                Console.WriteLine(Path.GetFileName(file));
            }

            foreach (var directory in Directory.GetDirectories(_directory))
            {
                Console.WriteLine(directory);
            }
        }
        
        public void DeleteFile(IDictionary<string, string> param)
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
            else Console.WriteLine("{0} does not exist", file);
        }
    }


Example Output
=====

    > echo "hello world!"
    hello world!
    > cd c:\test
    c:\test
    > ls
    file1.txt
    file2.txt
    file3.txt
    > del file1.txt
    > ls
    file2.txt
    file3.txt
    > exit


Future Plans
===

1. Allow the actions to have access to the cmdR settings so we can modify them on the fly (register/unregister routes, change the command prompt, exit codes, etc)
2. Automatic handling of help or ? command, so cmdR will list all the routes and there parameters along with a description
3. Startup message, so users can enter a message that will be displayed when cmdR.Run is called for the first time
4. Remove the dependency on the Console.Write and Console.ReadLine to output and read text so we can use cmdR within other types of application (i.e. WPF, Forms or Web pages(?))
5. Maybe move to an MVC type framework which will allow you to return results which could forward you onto other routes allowing you to chain commands
