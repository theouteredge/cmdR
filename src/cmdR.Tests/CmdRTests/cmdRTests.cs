using cmdR.CommandParsing;
using cmdR.IO;
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
        public void ExecuteCommand_CanExecuteSimpleCommand()
        {
            var cmdR = new CmdR("", new string[0]);
            var closureI = 1;

            cmdR.RegisterRoute("ls", (p, c, s) => { closureI += 1; });
            cmdR.ExecuteCommand("ls");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithSingleParam()
        {
            var cmdR = new CmdR("", new string[0]);
            var closureI = 1;

            cmdR.RegisterRoute("ls path", (p, c, s) => { closureI += 1; });
            cmdR.ExecuteCommand("ls c:\\programfiles\\msbob\\");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithEscappedParam()
        {
            var cmdR = new CmdR("", new string[0]);
            var closureI = 1;

            cmdR.RegisterRoute("ls path", (p, c, s) => { closureI += 1; });
            cmdR.ExecuteCommand("ls \"c:\\program files\\msbob\\\"");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithOptionalParam_NotSpecififed()
        {
            var cmdR = new CmdR("", new string[0]);
            var closureI = 1;

            cmdR.RegisterRoute("ls path filter?", (p, c, s) => { closureI += 1; });
            cmdR.ExecuteCommand("ls \"c:\\program files\\msbob\\\"");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithOptionalParam_Specified()
        {
            var cmdR = new CmdR("", new string[0]);
            var closureI = 1;

            cmdR.RegisterRoute("ls path filter?", (p, c, s) => { closureI += 1; });
            cmdR.ExecuteCommand("ls \"c:\\program files\\msbob\\\" *.exe");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_AllParamsArePassedCorreclty_Count()
        {
            var cmdR = new CmdR("", new string[0]);
            var paramCount = 0;

            cmdR.RegisterRoute("ls path filter?", (p, c, s) => { paramCount = p.Count; });
            cmdR.ExecuteCommand("ls \"c:\\program files\\msbob\\\" *.exe");

            Assert.AreEqual(2, paramCount);
        }

        [Test]
        public void ExecuteCommand_AllParamsArePassedCorreclty_Values()
        {
            var cmdR = new CmdR("", new string[0]);
            var validParams = 0;

            cmdR.RegisterRoute("ls path filter?", (p, c, s) => 
                {
                    if (p["path"] == "c:\\program files\\msbob\\")
                        validParams += 1;

                    if (p["filter"] == "*.exe")
                        validParams += 1;
                });

            cmdR.ExecuteCommand("ls \"c:\\program files\\msbob\\\" *.exe");

            Assert.AreEqual(2, validParams);
        }

        [Test]
        public void ExecuteCommand_AllParamsArePassedCorreclty_MissingOptionalParam()
        {
            var cmdR = new CmdR("", new string[0]);
            var validParams = 0;

            cmdR.RegisterRoute("ls path filter? param3?", (p, c, s) =>
                {
                    if (p["path"] == "c:\\program files\\msbob\\")
                        validParams += 1;

                    if (p["filter"] == "*.exe")
                        validParams += 1;

                    if (!p.ContainsKey("param3"))
                        validParams += 1;
                });

            cmdR.ExecuteCommand("ls \"c:\\program files\\msbob\\\" *.exe");

            Assert.AreEqual(3, validParams);
        }


        [Test]
        public void Run_IgnoresEmptyCommandLineArgs()
        {
            //todo: use moq to fake the console & state
            var console = new FakeCmdRConsole("");

            var cmdR = new CmdR(new OrderedCommandParser(), new Routing(), new RouteParser(), console, new CmdRState());

            cmdR.RegisterRoute("cd", (p, con, s) => con.Write("hello!"));

            cmdR.Run(new string[] { "" });


            Assert.AreEqual("> ", console.ConsoleWindow[0]);
        }


        [Test]
        public void Run_PrintsOutTheCommandPromptAfterRunningACommand()
        {
            //todo: use moq to fake the console & state
            var console = new FakeCmdRConsole("");

            var cmdR = new CmdR(new OrderedCommandParser(), new Routing(), new RouteParser(), console, new CmdRState());

            cmdR.RegisterRoute("cd", (p, con, s) => con.Write("hello!"));

            cmdR.Run(new string[] { "cd" });

            Assert.AreEqual("> ", console.ConsoleWindow[1]);
        }


        [Test]
        public void Run_ExecutesACommandPassedInOnTheCommandLine()
        {
            //todo: use moq to fake the console & state
            var console = new FakeCmdRConsole("");

            var cmdR = new CmdR(new OrderedCommandParser(), new Routing(), new RouteParser(), console, new CmdRState());

            cmdR.RegisterRoute("cd", (p, con, s) => con.Write("hello!"));

            cmdR.Run(new string[] { "cd" });

            Assert.AreEqual("hello!", console.ConsoleWindow[0]);
        }


        [Test]
        public void Run_WeCanModifyTheCommandPromptAndItsWrittenToTheConsole()
        {
            //todo: use moq to fake the console & state
            var console = new FakeCmdRConsole("");

            var cmdR = new CmdR(new OrderedCommandParser(), new Routing(), new RouteParser(), console, new CmdRState());

            cmdR.RegisterRoute("cd", (p, con, state) =>
                {
                    state.CmdPrompt = "new prompt> ";
                });

            cmdR.Run(new string[] { "cd" });

            Assert.AreEqual("new prompt> ", console.ConsoleWindow[0]);
        }


        [Test]
        public void Run_Help()
        {
            //todo: use moq to fake the console & state
            var console = new FakeCmdRConsole("");

            var cmdR = new CmdR(new OrderedCommandParser(), new Routing(), new RouteParser(), console, new CmdRState());

            cmdR.RegisterRoute("cd", (p, con, state) =>
                {
                    state.CmdPrompt = "new prompt> ";
                });

            cmdR.Run(new string[] { "help" });

            Assert.AreEqual("    help", console.ConsoleWindow[0]);
            Assert.AreEqual("\n    lists all the commands\n", console.ConsoleWindow[1]);
            Assert.AreEqual("    cd", console.ConsoleWindow[2]);
            Assert.AreEqual("\n", console.ConsoleWindow[3]);
            Assert.AreEqual("> ", console.ConsoleWindow[4]);
        }


        //todo: test the state that we can modify the command prompt
    }

    public class FakeCmdRConsole : ICmdRConsole
    {
        public List<string> ConsoleWindow { get; set; }
        public string TextToRead { get; set; }

        private bool _first = true;

        public FakeCmdRConsole(string textToRead)
        {
            ConsoleWindow = new List<string>();
            TextToRead = textToRead;
        }

        public string ReadLine()
        {
            if (_first)
            {
                _first = false;
                return TextToRead;
            }
            else return "exit";
        }

        public void Write(string line, params object[] parameters)
        {
            ConsoleWindow.Add(string.Format(line, parameters));
        }

        public void Write(string line)
        {
            ConsoleWindow.Add(string.Format(line));
        }

        public void WriteLine(string line, params object[] parameters)
        {
            ConsoleWindow.Add(string.Format(line, parameters));
        }

        public void WriteLine(string line)
        {
            ConsoleWindow.Add(string.Format(line));
        }
    }
}
