using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR.IO
{
    public interface ICmdRConsole
    {
        string ReadLine();

        void Write(string line, params string[] parameters);
        void Write(string line);

        void WriteLine(string line, params string[] paramters);
        void WriteLine(string line);
    }
}
