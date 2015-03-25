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
        readonly IRouteCommands _routing = new Routing();
        static readonly Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> DefaultAction = (p, c, s) => { };

        List<IRoute> _routes = new List<IRoute> {
            new Route("ls", new Dictionary<string, ParameterType>() 
                {
                    { "path", ParameterType.Required }, { "filter", ParameterType.Optional }
                }, DefaultAction)
        };


        [TestFixtureSetUp]
        public void Setup()
        {
            foreach(var route in _routes)
                _routing.RegisterRoute(route);
        }


        [Test]
        public void Parse_KeyValueCommandParser_HandlesSwitchesAndRoutesTheCommandCorrectly()
        {
            // Arrange
            var parser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls path \"c:\\ProgramFiles\" filter \"*.csv\" /s /switch", out commandName);
            var route = _routing.FindRoute(commandName, param);

            // Assert
            Assert.AreEqual("ls", commandName);
            Assert.AreEqual(4, param.Count); // two param and 2 switches

            Assert.IsNotNull(route);
            Assert.AreEqual("ls", route.Name);
        }

        [Test]
        public void Parse_KeyValueCommandParser_AddsSwitchesToTheParamsCorrectly()
        {
            // Arrange
            var parser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls path \"c:\\ProgramFiles\" filter \"*.csv\" /s /switch", out commandName);
            var route = _routing.FindRoute(commandName, param);

            // Assert
            Assert.IsTrue(param.ContainsKey("/s"));
            Assert.IsTrue(param.ContainsKey("/switch"));

            Assert.IsNotNull(route);
            Assert.AreEqual("ls", route.Name);
        }




        


        [Test]
        public void Parse_OrderedCommandParser_HandlesSwitchesAndRoutesTheCommandCorrectly()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            var commandName = "";

            parser.SetRoutes(_routing.GetRoutes());

            // Act
            var param = parser.Parse("ls c:\\ProgramFiles *.csv /s /switch", out commandName);
            var route = _routing.FindRoute(commandName, param);

            // Assert
            Assert.AreEqual("ls", commandName);
            Assert.AreEqual(4, param.Count); // two param and 2 switches

            Assert.IsNotNull(route);
            Assert.AreEqual("ls", route.Name);
        }

        [Test]
        public void Parse_OrderedCommandParser_AddsSwitchesToTheParamsCorrectly()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            
            var commandName = "";
            parser.SetRoutes(_routing.GetRoutes());

            // Act
            var param = parser.Parse("ls c:\\ProgramFiles *.csv /s /switch", out commandName);
            var route = _routing.FindRoute(commandName, param);

            // Assert
            Assert.IsTrue(param.ContainsKey("/s"));
            Assert.IsTrue(param.ContainsKey("/switch"));

            Assert.IsNotNull(route);
            Assert.AreEqual("ls", route.Name);
        }
    }
}
