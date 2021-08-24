using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FileShareProcessor.Service
{
    public interface IFileProcessService
    {
        Task<List<string>> GetFileNames();
        Task<bool> ProcessFile(string fileName);
    }

    public class FileProcessService : IFileProcessService
    {
        private readonly IFileShareService _fileShareService;
        private readonly IPatternService _patternService;

        public FileProcessService(IFileShareService fileShareService, IPatternService patternService)
        {
            _fileShareService = fileShareService;
            _patternService = patternService;
        }

        [ExcludeFromCodeCoverage]
        public async Task<List<string>> GetFileNames()
        {
            return await _fileShareService.GetFileNames();
        }

        public async Task<bool> ProcessFile(string fileName)
        {
            var isFileTextMatchesThePattern = await _patternService.IsFileTextMatchesThePattern(fileName);

            if (isFileTextMatchesThePattern)
            {
                return await _fileShareService.MoveFileFromInputToOutputDirectory(fileName);
            }

            return await _fileShareService.DeleteFileFromInputDirectory(fileName);
        }
    }
}
