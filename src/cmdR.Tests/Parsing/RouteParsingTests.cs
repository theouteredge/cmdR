using cmdR.Exceptions;
using NUnit.Framework;
using System.Linq;

namespace cmdR.Tests.Parsing
{
    [TestFixture]
    public class RouteParsingTests
    {
        [Test]
        public void Parse_CanParseSimpleRouteWithOnlyACommandName()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            parser.Parse("ls", out commandName);

            // Assert
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_CanParseSimpleRouteWithLeadingSpace()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            parser.Parse("  ls", out commandName);

            // Assert
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_CanParseSimpleRouteWithTrailingSpace()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            parser.Parse("ls  ", out commandName);

            // Assert
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_RouteWithSingleParameter()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls filter", out commandName);

            // Assert
            Assert.AreEqual("ls", commandName);
            Assert.AreEqual(1, param.Count);
            Assert.AreEqual("filter", param.First().Key);
            Assert.AreEqual(ParameterType.Required, param.First().Value);
        }

        [Test]
        public void Parse_RouteWithSingleOptionalParameter()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls filter?", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count);
            Assert.AreEqual("filter", param.First().Key);
            Assert.AreEqual(ParameterType.Optional, param.First().Value);
        }

        [Test]
        public void Parse_RouteWithTwoOptionalParameters()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls filter? path?", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count);
            
            Assert.AreEqual("filter", param.First().Key);
            Assert.AreEqual(ParameterType.Optional, param.First().Value);

            Assert.AreEqual("path", param.Skip(1).Take(1).Single().Key);
            Assert.AreEqual(ParameterType.Optional, param.Skip(1).Take(1).Single().Value);
        }

        [Test]
        public void Parse_RouteWithTwoRequiredParameters()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls filter path", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count);

            Assert.AreEqual("filter", param.First().Key);
            Assert.AreEqual(ParameterType.Required, param.First().Value);

            Assert.AreEqual("path", param.Skip(1).Take(1).Single().Key);
            Assert.AreEqual(ParameterType.Required, param.Skip(1).Take(1).Single().Value);
        }

        [Test]
        public void Parse_RouteWithOneRequiredAndOneOptinalParameters()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls filter path?", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count);

            Assert.AreEqual("filter", param.First().Key);
            Assert.AreEqual(ParameterType.Required, param.First().Value);

            Assert.AreEqual("path", param.Skip(1).Take(1).Single().Key);
            Assert.AreEqual(ParameterType.Optional, param.Skip(1).Take(1).Single().Value);
        }

        [Test]
        [ExpectedException(typeof(InvalidRouteException))]
        public void Parse_RouteWithOneRequiredAndOneOptinalInTheWrongOrder()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse("ls filter? path", out commandName);
        }

        [Test]
        [ExpectedException(typeof(InvalidRouteException))]
        public void Parse_EmptyRoute()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse("", out commandName);
        }

        [Test]
        [ExpectedException(typeof(InvalidRouteException))]
        public void Parse_NullRoute()
        {
            // Arrange
            var parser = new RouteParser();
            var commandName = "";

            // Act
            var param = parser.Parse(null, out commandName);
        }
    }
}
