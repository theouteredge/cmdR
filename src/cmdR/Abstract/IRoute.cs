using cmdR.IO;
using System;
using System.Collections.Generic;

namespace cmdR.Abstract
{
    public interface IRoute
    {
        string Name { get; }
        string Description { get; }

        void Execute(IDictionary<string, string> parameters, ICmdRConsole console, ICmdRState state);
        bool Match(List<string> paramNames);

        List<string> GetParmaNames();
        IDictionary<string, ParameterType> GetParameters();
        
        int RequiredParametersCount();
        int TotalParametersCount();
    }
}
