using System;

namespace cmdR.IO
{
    public class CmdRConsole : ICmdRConsole
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Write(string line, object[] parameters = null)
        {
            Console.Write(line, parameters);
        }

        public void Write(string line)
        {
            Console.Write(line);
        }

        public void WriteLine(string line, object[] paramters = null)
        {
            Console.WriteLine(line, paramters);
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}