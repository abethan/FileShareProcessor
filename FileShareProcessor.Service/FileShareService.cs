using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace FileShareProcessor.Service
{
    public interface IFileShareService
    {
        Task<List<string>> GetFileNames();
        IAsyncEnumerable<string> ReadFileLines(string fileName);
        Task<bool> MoveFileFromInputToOutputDirectory(string fileName);
        Task<bool> DeleteFileFromInputDirectory(string fileName);
    }

    [ExcludeFromCodeCoverage]
    public class FileShareService : IFileShareService
    {
        private readonly ShareClient _shareClient;
        private readonly ShareDirectoryClient _inputDirectoryClient;
        private readonly ShareDirectoryClient _outputDirectoryClient;

        public FileShareService(IOptions<FunctionAppSettings> functionAppSettings)
        {
            var settings = functionAppSettings.Value;

            _shareClient = new ShareClient(settings.StorageConnectionString, settings.FileShareName);
            _inputDirectoryClient = _shareClient.GetDirectoryClient(settings.InputDirectoryName);
            _outputDirectoryClient = _shareClient.GetDirectoryClient(settings.OutputDirectoryName);
        }

        public async Task<List<string>> GetFileNames()
        {
            var filesAndDirectories = _inputDirectoryClient.GetFilesAndDirectoriesAsync();

            var fileNames = new List<string>();
            await foreach (Page<ShareFileItem> page in filesAndDirectories.AsPages())
            {
                foreach (ShareFileItem shareFileItem in page.Values)
                {
                    fileNames.Add(shareFileItem.Name);
                }
            }

            return fileNames;
        }

        public async IAsyncEnumerable<string> ReadFileLines(string fileName)
        {
            var shareFileClient = _inputDirectoryClient.GetFileClient(fileName);

            using var stream = await shareFileClient.OpenReadAsync();
            using StreamReader sr = new StreamReader(stream);

            while (sr.Peek() >= 0)
            {
                yield return sr.ReadLine();
            }
        }

        public async Task<bool> MoveFileFromInputToOutputDirectory(string fileName)
        {
            var sourceShareFileClient = _inputDirectoryClient.GetFileClient(fileName);
            var destinationShareFileClient = _outputDirectoryClient.GetFileClient(fileName);

            await destinationShareFileClient.StartCopyAsync(sourceShareFileClient.Uri);

            if (await destinationShareFileClient.ExistsAsync())
            {
                await sourceShareFileClient.DeleteAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteFileFromInputDirectory(string fileName)
        {
            var sourceShareFileClient = _inputDirectoryClient.GetFileClient(fileName);
            var resposne = await sourceShareFileClient.DeleteIfExistsAsync();

            return resposne.Value;
        }
    }
}
