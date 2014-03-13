using System.Linq;
using cmdR.CommandParsing;
using cmdR.Exceptions;
using NUnit.Framework;

namespace cmdR.Tests.Parsing
{
    [TestFixture]
    public class KeyValueCommandParserTests
    {
        [Test]
        public void Parse_CommandNameOnly()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse("ls", out commandName);

            // Assert
            Assert.AreEqual(0, param.Count());
            Assert.AreEqual("ls", commandName);
        }


        [Test]
        public void Parse_CommandNameOnlyWithLeadingSpace()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse(" ls", out commandName);

            // Assert
            Assert.AreEqual(0, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_CommandNameOnlyWithTrailingSpace()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse("ls ", out commandName);

            // Assert
            Assert.AreEqual(0, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_WithSingleKeyValueParameter()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse(@"ls path c:\", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_SingleParamIsSplitCorrectly()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse(@"ls path c:\", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());  
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual(@"c:\", param.Values.First());
        }

        [Test]
        public void Parse_WithTwoKeyValueParameters()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse(@"ls path c:\ filter *.csv", out commandName);

            // Assert
            Assert.AreEqual(2, param.Count());
            Assert.AreEqual("ls", commandName);
        }

        [Test]
        public void Parse_TwoParamAreSplitCorrectly()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse(@"ls path c:\ filter *.csv", out commandName);

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
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse("ls path \"c:\\Program Files\"", out commandName);

            // Assert
            Assert.AreEqual(1, param.Count());
            Assert.AreEqual("path", param.Keys.First());
            Assert.AreEqual("c:\\Program Files", param.Values.First());
        }

        [Test]
        public void Parse_TwoEscappedParam()
        {
            // Arrange
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse("ls path \"c:\\Program Files\" filter \"*.csv\"", out commandName);

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
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse("ls path \"c:\\Program Files\" filter *.csv", out commandName);

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
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse("ls path c:\\ProgramFiles filter \"*.csv\"", out commandName);

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
            var kvParser = new KeyValueCommandParser();
            var commandName = "";

            // Act
            var param = kvParser.Parse("ls path c:\\ProgramFiles filter \"*.csv", out commandName);
        }
    }
}
