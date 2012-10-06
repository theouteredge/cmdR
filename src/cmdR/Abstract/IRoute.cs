using System;
using System.Collections.Generic;
namespace cmdR
{
    interface IRoute
    {
        string Name { get; set; }

        void Execute(IDictionary<string, string> parameters);
        bool Match(List<string> paramNames);
    }
}
