using cmdR.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public interface ICmdRState
    {
        string CmdPrompt { get; set; }
        string[] ExitCodes { get; set; }

        IDictionary<string, object> Variables { get; set; }
        List<IRoute> Routes { get; set; }
    }
}
