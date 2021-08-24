using FileShareProcessor.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileShareProcessor
{
    public class ActivityFunctons
    {
        private readonly IFileProcessService _fileProcessService;

        public ActivityFunctons(IFileProcessService fileProcessService)
        {
            _fileProcessService = fileProcessService;
        }

        [FunctionName(nameof(GetTimerMinutesInterval))]
        public int GetTimerMinutesInterval([ActivityTrigger] object input)
        {
            var timerHoursInterval = Environment.GetEnvironmentVariable("TimerMinutesInterval");
            return int.Parse(timerHoursInterval);
        }

        [FunctionName(nameof(GetFileNames))]
        public async Task<List<string>> GetFileNames([ActivityTrigger] object input)
        {
            return await _fileProcessService.GetFileNames();
        }

        [FunctionName(nameof(ProcessFiles))]
        public async Task<bool> ProcessFiles([ActivityTrigger] string fileName)
        {
            return await _fileProcessService.ProcessFile(fileName);
        }
    }
}