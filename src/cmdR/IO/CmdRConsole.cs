using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.IO
{
    public class CmdRConsole : ICmdRConsole
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Write(string line, string[] parameters = null)
        {
            Console.Write(line, parameters);
        }

        public void Write(string line)
        {
            Console.Write(line);
        }

        public void WriteLine(string line, string[] paramters = null)
        {
            Console.WriteLine(line, paramters);
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}