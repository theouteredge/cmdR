cmdR
====

A simple routing system for command line applications, allowing a simple and structured way of taking in multiple 
commands and associating these command with Actions to do useful stuff

Usage
=====

    class Program
    {
        static void Main(string[] args)
        {
            var example = new Example();
            var cmdR = new CmdR();
            
            cmdR.RegisterRoute("cd path", example.ChangeDirectory);
            cmdR.RegisterRoute("ls path? filter?", example.ListDirectory);
            cmdR.RegisterRoute("del file", example.DeleteFile);
            
            cmdR.Run(args);
        }
    }
    
    public class Example()
    {
        public void ChangeDirectory(IDictionary<string, string> params)
        {
            // do cool stuff
            Console.WriteLine(params["path"]);
        }
        
        public void ListDirectory(IDictionary<string, string> params)
        {
            // do cool stuff
        }
        
        public void DeleteFile(IDictionary<string, string> params)
        {
            // do cool stuff
        }
    }
