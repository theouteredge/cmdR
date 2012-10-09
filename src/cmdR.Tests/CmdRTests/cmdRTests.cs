using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.Tests.CmdRTests
{
    [TestFixture]
    public class cmdRTests
    {
        [Test]
        public void ExecuteCommand_CanExecuteCommand()
        {
            var cmdR = new CmdR();
            var closureI = 1;
            
            cmdR.RegisterRoute("ls path", (p) => { closureI += 1; });
            cmdR.ExecuteCommand("ls path \"c:\\program files\\msbob\\\"");

            Assert.AreEqual(2, closureI);
        }
    }
}
