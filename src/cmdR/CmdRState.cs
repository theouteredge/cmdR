using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmdR
{
    public class CmdRState : ICmdRState
    {
        public string CmdPrompt { get; set; }
        public string[] ExitCodes { get; set; }
    }
}