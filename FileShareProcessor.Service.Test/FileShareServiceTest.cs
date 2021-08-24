using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FileShareProcessor.Service.Test
{
    public class FileShareServiceTest
    {
        private readonly Mock<IFileShareService> _fileShareService;

        public FileShareServiceTest()
        {
            _fileShareService = new Mock<IFileShareService>();
        }

        [Fact]
        public async Task GetFiles()
        {
            _fileShareService.Setup(x => x.GetFileNames()).ReturnsAsync(new List<string> { "Test.txt", "Test1.txt" });
            var fileNames = await _fileShareService.Object.GetFileNames();
            Assert.True(fileNames.Count == 2);
        }

        [Fact]
        public async Task ReadLine()
        {
            string[] returnStringArray = { "Test1", "Test2" };
            _fileShareService.Setup(x => x.ReadFileLines(It.IsAny<string>())).Returns(returnStringArray.GetAsyncEnumerable());
            List<string> lines = new List<string>();
            await foreach (var line in _fileShareService.Object.ReadFileLines("Test.txt"))
            {
                lines.Add(line);
            }

            Assert.True(lines.Count > 0);
        }

        [Fact]
        public async Task MoveFile()
        {
            _fileShareService.Setup(x => x.MoveFileFromInputToOutputDirectory(It.IsAny<string>())).ReturnsAsync(true);
            var result = await _fileShareService.Object.MoveFileFromInputToOutputDirectory("ToDIscuss_AKila.txt");
            Assert.True(result);
        }
    }
}
