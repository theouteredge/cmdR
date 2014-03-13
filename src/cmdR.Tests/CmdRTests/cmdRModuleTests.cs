using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cmdR.IO;

namespace cmdR.Tests.CmdRTests
{
    [TestFixture]
    public class CmdRModuleTests
    {
        [Test]
        public void AutoRegisterCommands_LoadsUpBothModulesAndAllCommandsAreRegistered()
        {
            var cmdR = new CmdR("> ");
            cmdR.AutoRegisterCommands();

            // 1 built in route, 2 basic module commands, 2 attribute commands
            Assert.AreEqual(5, cmdR.State.Routes.Count);
        }

        [Test]
        public void AutoRegisterCommands_BasicCommandsAreRegistered()
        {
            var cmdR = new CmdR("> ");
            cmdR.AutoRegisterCommands();

            Assert.AreEqual(true, cmdR.State.Routes.Any(x => x.Name == "basic-module1"));
            Assert.AreEqual(true, cmdR.State.Routes.Any(x => x.Name == "basic-module2"));
        }

        [Test]
        public void AutoRegisterCommands_AttributeCommandAreRegistered()
        {
            var cmdR = new CmdR("> ");
            cmdR.AutoRegisterCommands();

            Assert.AreEqual(true, cmdR.State.Routes.Any(x => x.Name == "attribute-module1"));
            Assert.AreEqual(true, cmdR.State.Routes.Any(x => x.Name == "attribute-module2"));
        }

        [Test]
        public void AutoRegisterCommands_CanInvokeBasicModuleRouteOne()
        {
            var fakeConsole = new FakeCmdRConsole("");

            var cmdR = new CmdR(console: fakeConsole);
            cmdR.AutoRegisterCommands();
            cmdR.Run(new string[] { "basic-module1" });


            Assert.AreEqual("module-test1", fakeConsole.ConsoleWindow[0]);
        }

        [Test]
        public void AutoRegisterCommands_CanInvokeBasicModuleRouteTwo()
        {
            var fakeConsole = new FakeCmdRConsole("");

            var cmdR = new CmdR(console: fakeConsole);
            cmdR.AutoRegisterCommands();
            cmdR.Run(new string[] { "basic-module2" });


            Assert.AreEqual("module-test2", fakeConsole.ConsoleWindow[0]);
        }

        [Test]
        public void AutoRegisterCommands_CanInvokeAttributeModuleRouteOne()
        {
            var fakeConsole = new FakeCmdRConsole("");

            var cmdR = new CmdR(console: fakeConsole);
            cmdR.AutoRegisterCommands();
            cmdR.Run(new string[] { "attribute-module1" });


            Assert.AreEqual("attribute-test1", fakeConsole.ConsoleWindow[0]);
        }

        [Test]
        public void AutoRegisterCommands_CanInvokeAttributeModuleRouteTwo()
        {
            var fakeConsole = new FakeCmdRConsole("");

            var cmdR = new CmdR(console: fakeConsole);
            cmdR.AutoRegisterCommands();
            cmdR.Run(new string[] { "attribute-module2" });


            Assert.AreEqual("attribute-test2", fakeConsole.ConsoleWindow[0]);
        }
    }




    public class BasicTestModule :ICmdRModule
    {
        private readonly CmdR _cmdR;

        public BasicTestModule(CmdR cmdR)
        {
            _cmdR = cmdR;

            _cmdR.RegisterRoute("basic-module1", MethodOne, "Basic Echo Command");
            _cmdR.RegisterRoute("basic-module2", MethodTwo, "Basic Echo Command");
        }

        private void MethodOne(IDictionary<string, string> arg1, CmdR cmdR)
        {
            cmdR.Console.WriteLine("module-test1");
        }

        private void MethodTwo(IDictionary<string, string> arg1, CmdR cmdR)
        {
            cmdR.Console.WriteLine("module-test2");
        }
    }


    public class AttributeTestModule : ICmdRModule
    {
        private readonly CmdR _cmdR;

        public AttributeTestModule(CmdR cmdR)
        {
            _cmdR = cmdR;
        }

        [CmdRoute("attribute-module1", "...description...", false)]
        public void MethodOne(IDictionary<string, string> arg1, CmdR cmdR)
        {
            cmdR.Console.WriteLine("attribute-test1");
        }

        [CmdRoute("attribute-module2", "...description...", false)]
        public void MethodTwo(IDictionary<string, string> arg1, CmdR cmdR)
        {
            cmdR.Console.WriteLine("attribute-test2");
        }
    }
}
