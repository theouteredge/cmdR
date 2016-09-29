using cmdR.Abstract;
using System.Collections.Generic;

namespace cmdR
{
    public class CmdRState : ICmdRState
    {
        public string CmdPrompt { get; set; }
        public string[] ExitCodes { get; set; }
        
        public IDictionary<string, object> Variables { get; set; }
        public List<IRoute> Routes { get; set; }

        public CmdRState()
        {
            Variables = new Dictionary<string, object>();
        }
    }
}