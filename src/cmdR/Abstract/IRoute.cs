using cmdR.IO;
using System;
using System.Collections.Generic;

namespace cmdR.Abstract
{
    interface IRoute
    {
        string Name { get; set; }

        void Execute(IDictionary<string, string> parameters, ICmdRConsole console, ICmdRState state);
        bool Match(List<string> paramNames);
    }
}
