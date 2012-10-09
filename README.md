cmdR
====

A simple command routing system for command line applications, giing you a simple way of routing commands to an Actions to perform your apps logic.
CmdR is based on command loop where the user enters a command with params, cmdR will execute a commands Action and then wait for the user to enter another command.


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

            // creating the CmdR class passing in the command prompt to use and a list of exit codes the user can type to exit the cmdR loop
            // these are the system defaults, so they dont actually need to be passed in
            var cmdR = new CmdR("> ", new string[] { "exit" });
            
            // setting up the command routes with the actions which should be executed for each route
            cmdR.RegisterRoute("cd path", example.ChangeDirectory);
            cmdR.RegisterRoute("ls filter?", example.ListDirectory);
            cmdR.RegisterRoute("del file", example.DeleteFile);

            // registering a route with a lambda
            cmdR.RegisterRoute("echo text", (parameters) => 
                { 
                    Console.WriteLine(parameters["text"]);
                }));

            
            // start the cmdR loop passing in the args as the first command to execute
            cmdR.Run(args);
        }
    }
    
    public class DOSPromptReplication()
    {
        private string _directory = "c:\";

        public void ChangeDirectory(IDictionary<string, string> params)
        {
            _directoty = params["path"];
            Console.WriteLine(params["path"]);
        }
        
        public void ListDirectory(IDictionary<string, string> params)
        {
            // loops through all the files in a directory and lists each one on a new line
        }
        
        public void DeleteFile(IDictionary<string, string> params)
        {
            // deletes a file from the current directoty
        }
    }

Output
======

    > echo "hello world!
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
