using cmdR.CommandParsing;
using cmdR.Exceptions;
using cmdR.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.Tests.Parsing
{
    [TestFixture]
    public class OrderedCommandParserTests
    {
        static Action<IDictionary<string, string>, ICmdRConsole, ICmdRState> _defaultAction = (p, c, s) => { var i = 1 + 1; };

        List<Route> _simpleRoute = new List<Route> {
            new Route("ls", new Dictionary<string, ParameterType>(), _defaultAction)
        };

        List<Route> _singleParamRoute = new List<Route> {
            new Route("ls", new Dictionary<string, ParameterType>() {{ "path", ParameterType.Required }}, _defaultAction)
        };

        List<Route> _twoParamRoute = new List<Route> {
            new Route("ls", new Dictionary<string, ParameterType>() 
            {
                { "path", ParameterType.Required }, { "filter", ParameterType.Required }
            }, _defaultAction)
        };

        List<Route> _twoParamWithOptionalRoute = new List<Route> {
            new Route("ls", new Dictionary<string, ParameterType>() 
            {
                { "path", ParameterType.Required }, { "filter", ParameterType.Optional }
            }, _defaultAction)
        };


        [Test]
        public void Parse_CommandNameOnly()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_simpleRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls", out commandName);

            // Assert
            Assert.AreEqual(0, param.Count());
            Assert.AreEqual("ls", commandName);
        }


        [Test]
        public void Parse_CommandNameOnlyWithLeadingSpace()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_simpleRoute);

            var commandName = "";

            // Act
            var param = parser.Parse(" ls", out commandName);

            // Assert
            Assert.AreEqual(0, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_CommandNameOnlyWithTrailingSpace()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_simpleRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls ", out commandName);

            // Assert
            Assert.AreEqual(0, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_WithSingleKeyValueParameter()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_singleParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse(@"ls c:\", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_SingleParamIsSplitCorrectly()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_singleParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse(@"ls c:\", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual(@"c:\", param.Values.First());
        }

        [Test]
        public void Parse_WithTwoParameters()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_twoParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse(@"ls c:\ *.csv", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_WithTwoParamsOptionalNotSpecified()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_twoParamWithOptionalRoute);

            var commandName = "";

            // Act
            var param = parser.Parse(@"ls c:\", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_TwoParamAreSplitCorrectly()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_twoParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse(@"ls c:\ *.csv", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual(@"c:\", param.Values.First());

            Assert.AreEqual("filter", param.Keys.Skip(1).Take(1).Single());
            Assert.AreEqual(@"*.csv", param.Values.Skip(1).Take(1).Single());
        }


        [Test]
        public void Parse_SingleEscappedParam()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_singleParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls \"c:\\Program Files\"", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual("c:\\Program Files", param.Values.First());
        }

        [Test]
        public void Parse_ParamsGetExtraSpacesTrimmed()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_singleParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls  c:\\ProgramFiles\\", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual("c:\\ProgramFiles\\", param.Values.First());
        }

        [Test]
        public void Parse_TwoEscappedParam()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_twoParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls \"c:\\Program Files\" \"*.csv\"", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual("c:\\Program Files", param.Values.First());

            Assert.AreEqual("filter", param.Keys.Skip(1).Take(1).Single());
            Assert.AreEqual(@"*.csv", param.Values.Skip(1).Take(1).Single());
        }

        [Test]
        public void Parse_TwoParamsFirstOneEscapped()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_twoParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls \"c:\\Program Files\" *.csv", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual("c:\\Program Files", param.Values.First());

            Assert.AreEqual("filter", param.Keys.Skip(1).Take(1).Single());
            Assert.AreEqual(@"*.csv", param.Values.Skip(1).Take(1).Single());
        }

        [Test]
        public void Parse_TwoParamsSecondOneEscapped()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_twoParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls c:\\ProgramFiles \"*.csv\"", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual("c:\\ProgramFiles", param.Values.First());

            Assert.AreEqual("filter", param.Keys.Skip(1).Take(1).Single());
            Assert.AreEqual(@"*.csv", param.Values.Skip(1).Take(1).Single());
        }


        [Test]
        [ExpectedException(typeof(InvalidCommandException))]
        public void Parse_TwoParamsSecondOneEscappedIncorrectlyAndThrowsAnException()
        {
            // Arrange
            var parser = new OrderedCommandParser();
            parser.SetRoutes(_twoParamRoute);

            var commandName = "";

            // Act
            var param = parser.Parse("ls c:\\ProgramFiles \"*.csv", out commandName);
        }

        [Test]
        [ExpectedException(typeof(NoRoutesSetupException))]
        public void Parse_TwoParamsButNoRoutesAndThrowsAnException()
        {
            // Arrange
            var parser = new OrderedCommandParser();

            var commandName = "";

            // Act
            var param = parser.Parse("ls c:\\ProgramFiles \"*.csv", out commandName);
        }
    }
}
