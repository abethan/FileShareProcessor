using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FileShareProcessor.Service.Test
{
    public class PatternServiceTest
    {
        private readonly Mock<IFileShareService> _fileShareService;

        public PatternServiceTest()
        {
            _fileShareService = new Mock<IFileShareService>();
        }

        [Fact]
        public void ConvertToRegexWithValidStringShoudReturnExpectedRegexPattern()
        {
            var patternService = new PatternService(_fileShareService.Object, Options.Create(new FunctionAppSettings { Pattern = "a*d" }));
            var pattern = patternService.ConvertPatternToRegex();

            Assert.Equal("^a.*d$", pattern);
        }

        [Fact]
        public void ConvertToRegexWithEmptyStringShoudReturnEmptyString()
        {
            var patternService = new PatternService(_fileShareService.Object, Options.Create(new FunctionAppSettings { Pattern = string.Empty }));
            var pattern = patternService.ConvertPatternToRegex();

            Assert.Equal(string.Empty, pattern);
        }

        [Fact]
        public void ConvertToRegexWithNullShoudReturnEmptyString()
        {
            var patternService = new PatternService(_fileShareService.Object, Options.Create(new FunctionAppSettings()));
            var pattern = patternService.ConvertPatternToRegex();

            Assert.Equal(string.Empty, pattern);
        }

        [Fact]
        public void IsFileTextMatchesThePatternWithValidPatternAndMatchingFirstLineShoudReturnTrue()
        {
            string[] returnLines = { "abcd", "abc" };
            PatternService patternService = SetupFileShareService(returnLines);

            Task<bool> result = patternService.IsFileTextMatchesThePattern("Text.txt");

            Assert.True(result.Result);
        }

        [Fact]
        public void IsFileTextMatchesThePatternWithValidPatternAndMatchingLastLineShoudReturnTrue()
        {
            string[] returnLines = { "abc", "a", "abcde", "abcd" };
            PatternService patternService = SetupFileShareService(returnLines);

            Task<bool> result = patternService.IsFileTextMatchesThePattern("Text.txt");

            Assert.True(result.Result);
        }

        [Fact]
        public void IsFileTextMatchesThePatternWithIValidPatternAndNotMatchingLinesShoudReturnFalse()
        {
            string[] returnLines = { "abc", string.Empty, "abcde", "bgcd" };
            PatternService patternService = SetupFileShareService(returnLines);

            Task<bool> result = patternService.IsFileTextMatchesThePattern("Text.txt");

            Assert.False(result.Result);
        }

        private PatternService SetupFileShareService(string[] returnLines)
        {
            _fileShareService.Setup(x => x.ReadFileLines(It.IsAny<string>())).Returns(returnLines.GetAsyncEnumerable());

            return new PatternService(_fileShareService.Object, Options.Create(new FunctionAppSettings { Pattern = "a*d" }));
        }
    }
}