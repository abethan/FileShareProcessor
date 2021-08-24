using System.Diagnostics.CodeAnalysis;

namespace FileShareProcessor.Service
{

    [ExcludeFromCodeCoverage]
    public class FunctionAppSettings
    {
        public string StorageConnectionString { get; set; }
        public string FileShareName { get; set; }
        public string InputDirectoryName { get; set; }
        public string OutputDirectoryName { get; set; }
        public string Pattern { get; set; }
    }
}
