﻿using NUnit.Framework;
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
            var cmdR = new CmdR();
            var closureI = 1;

            cmdR.RegisterRoute("ls", (p) => { closureI += 1; });
            cmdR.ExecuteCommand("ls");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithSingleParam()
        {
            var cmdR = new CmdR();
            var closureI = 1;

            cmdR.RegisterRoute("ls path", (p) => { closureI += 1; });
            cmdR.ExecuteCommand("ls path c:\\programfiles\\msbob\\");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithEscappedParam()
        {
            var cmdR = new CmdR();
            var closureI = 1;
            
            cmdR.RegisterRoute("ls path", (p) => { closureI += 1; });
            cmdR.ExecuteCommand("ls path \"c:\\program files\\msbob\\\"");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithOptionalParam_NotSpecififed()
        {
            var cmdR = new CmdR();
            var closureI = 1;

            cmdR.RegisterRoute("ls path filter?", (p) => { closureI += 1; });
            cmdR.ExecuteCommand("ls path \"c:\\program files\\msbob\\\"");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_CanExecuteCommandWithOptionalParam_Specified()
        {
            var cmdR = new CmdR();
            var closureI = 1;

            cmdR.RegisterRoute("ls path filter?", (p) => { closureI += 1; });
            cmdR.ExecuteCommand("ls path \"c:\\program files\\msbob\\\" filter *.exe");

            Assert.AreEqual(2, closureI);
        }

        [Test]
        public void ExecuteCommand_AllParamsArePassedCorreclty_Count()
        {
            var cmdR = new CmdR();
            var paramCount = 0;

            cmdR.RegisterRoute("ls path filter?", (p) => { paramCount = p.Count; });
            cmdR.ExecuteCommand("ls path \"c:\\program files\\msbob\\\" filter *.exe");

            Assert.AreEqual(2, paramCount);
        }

        [Test]
        public void ExecuteCommand_AllParamsArePassedCorreclty_Values()
        {
            var cmdR = new CmdR();
            var validParams = 0;

            cmdR.RegisterRoute("ls path filter?", (p) => 
                {
                    if (p["path"] == "c:\\program files\\msbob\\")
                        validParams += 1;

                    if (p["filter"] == "*.exe")
                        validParams += 1;
                });

            cmdR.ExecuteCommand("ls path \"c:\\program files\\msbob\\\" filter *.exe");

            Assert.AreEqual(2, validParams);
        }

        [Test]
        public void ExecuteCommand_AllParamsArePassedCorreclty_MissingOptionalParam()
        {
            var cmdR = new CmdR();
            var validParams = 0;

            cmdR.RegisterRoute("ls path filter? param3?", (p) =>
            {
                if (p["path"] == "c:\\program files\\msbob\\")
                    validParams += 1;

                if (p["filter"] == "*.exe")
                    validParams += 1;

                if (!p.ContainsKey("param3"))
                    validParams += 1;
            });

            cmdR.ExecuteCommand("ls path \"c:\\program files\\msbob\\\" filter *.exe");

            Assert.AreEqual(3, validParams);
        }
    }
}