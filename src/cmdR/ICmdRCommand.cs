using System.Collections.Generic;

namespace cmdR
{
    public interface ICmdRCommand
    {
        string Command { get; }
        string Description { get; }
        void Execute(IDictionary<string, string> param, CmdR cmdR);
    }
}