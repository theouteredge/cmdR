using cmdR.Abstract;
using System.Collections.Generic;

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
