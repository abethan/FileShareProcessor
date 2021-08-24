using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FileShareProcessor.Service.Test
{
    public class FileProcessServiceTest
    {
        private readonly Mock<IFileShareService> _fileShareService;
        private readonly Mock<IPatternService> _patternService;

        public FileProcessServiceTest()
        {
            _fileShareService = new Mock<IFileShareService>();
            _patternService = new Mock<IPatternService>();
        }

        [Fact]
        public async Task ProcessFileWithTextMatchesThePatternShouldMoveTheFile()
        {
            _patternService.Setup(x => x.IsFileTextMatchesThePattern(It.IsAny<string>())).ReturnsAsync(true);

            var fileProcessService = new FileProcessService(_fileShareService.Object, _patternService.Object);
            await fileProcessService.ProcessFile("test.txt");

            _fileShareService.Verify(x => x.MoveFileFromInputToOutputDirectory(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ProcessFileWithTextNotMatchesThePatternShouldDeleteTheFile()
        {
            _patternService.Setup(x => x.IsFileTextMatchesThePattern(It.IsAny<string>())).ReturnsAsync(false);

            var fileProcessService = new FileProcessService(_fileShareService.Object, _patternService.Object);
            await fileProcessService.ProcessFile("test.txt");

            _fileShareService.Verify(x => x.DeleteFileFromInputDirectory(It.IsAny<string>()), Times.Once);
        }
    }
}
