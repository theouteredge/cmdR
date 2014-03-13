using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cmdR.Abstract;
using cmdR.CommandParsing;
using cmdR.IO;
using NUnit.Framework;

namespace cmdR.Tests.Parsing
{
    [TestFixture]
    public class SwitchParsingTests
    {
        [Test]
        public void Parse_KeyValueCommandParser_HandlesSwitchesAndRoutesTheCommandCorrectly()
        {
            // Arrange
            var parser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls path \"c:\\ProgramFiles\" filter \"*.csv\" /s /switch", out commandName);    

            // Assert
            Assert.AreEqual("ls", commandName);
            Assert.AreEqual(4, param.Count); // two param and 2 switches
        }

        [Test]
        public void Parse_KeyValueCommandParser_AddsSwitchesToTheParamsCorrectly()
        {
            // Arrange
            var parser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls path \"c:\\ProgramFiles\" filter \"*.csv\" /s /switch", out commandName);

            // Assert
            Assert.IsTrue(param.ContainsKey("/s"));
            Assert.IsTrue(param.ContainsKey("/switch"));
        }




        static Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> _defaultAction = (p, c, s) => { };

        List<IRoute> _routes = new List<IRoute> {
            new Route("ls", new Dictionary<string, ParameterType>() 
                {
                    { "path", ParameterType.Required }, { "filter", ParameterType.Optional }
                }, _defaultAction)
        };


        [Test]
        public void Parse_OrderedCommandParser_HandlesSwitchesAndRoutesTheCommandCorrectly()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            var commandName = "";

            parser.SetRoutes(_routes);

            // Act
            var param = parser.Parse("ls c:\\ProgramFiles *.csv /s /switch", out commandName);

            // Assert
            Assert.AreEqual("ls", commandName);
            Assert.AreEqual(4, param.Count); // two param and 2 switches
        }

        [Test]
        public void Parse_OrderedCommandParser_AddsSwitchesToTheParamsCorrectly()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            var commandName = "";

            parser.SetRoutes(_routes);

            // Act
            var param = parser.Parse("ls c:\\ProgramFiles *.csv /s /switch", out commandName);

            // Assert
            Assert.IsTrue(param.ContainsKey("/s"));
            Assert.IsTrue(param.ContainsKey("/switch"));
        }
    }
}
