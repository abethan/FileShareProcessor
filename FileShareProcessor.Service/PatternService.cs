using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileShareProcessor.Service
{
    public interface IPatternService
    {
        string ConvertPatternToRegex();
        Task<bool> IsFileTextMatchesThePattern(string fileName);
    }

    public class PatternService : IPatternService
    {
        private readonly IFileShareService _fileShareService;
        private readonly IOptions<FunctionAppSettings> _functionAppSettings;
        private readonly string _regexPattern;

        public PatternService(IFileShareService fileShareService, IOptions<FunctionAppSettings> functionAppSettings)
        {
            _fileShareService = fileShareService;
            _functionAppSettings = functionAppSettings;
            _regexPattern = ConvertPatternToRegex();
        }

        public string ConvertPatternToRegex()
        {
            if (string.IsNullOrEmpty(_functionAppSettings.Value.Pattern)) return string.Empty;

            return "^" + Regex.Escape(_functionAppSettings.Value.Pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        public async Task<bool> IsFileTextMatchesThePattern(string fileName)
        {
            await foreach (var line in _fileShareService.ReadFileLines(fileName))
            {
                if (Regex.IsMatch(line, _regexPattern))
                {
                    return true;
                }
            }

            return false;
        }
    }
}