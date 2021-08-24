using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileShareProcessor
{
    public static class OrchestratorFunctions
    {
        [FunctionName(nameof(ProcessFileShareOrchestrator))]
        public static async Task<object> ProcessFileShareOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log = context.CreateReplaySafeLogger(log);

            log.LogInformation("About to call GetTimerMinutesInterval.");
            var timerMinutesInterval = await context.CallActivityWithRetryAsync<int>("GetTimerMinutesInterval", new RetryOptions(TimeSpan.FromSeconds(5), 3)
            {
                Handle = ex => ex is InvalidOperationException
            }, null);

            log.LogInformation($"TimerMinutesInterval :{timerMinutesInterval}hours");

            while (true)
            {
                DateTime deadline = context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(timerMinutesInterval));
                await context.CreateTimer(deadline, CancellationToken.None);

                try
                {
                    log.LogInformation("About to call GetFileNames.");
                    var fileNames = await context.CallActivityWithRetryAsync<List<string>>("GetFileNames", new RetryOptions(TimeSpan.FromSeconds(5), 3)
                    {
                        Handle = ex => ex is InvalidOperationException
                    }, null);

                    if (fileNames.Count == 0)
                    {
                        log.LogInformation("No files to process.");
                        continue;
                    }

                    var fileProcessTasks = new List<Task<bool>>();
                    log.LogInformation("About to call ProcessFiles in a loop.");
                    foreach (var fileName in fileNames)
                    {
                        log.LogInformation($"Calling ProcessFile for {fileName}");
                        var fileProcessTask = context.CallActivityWithRetryAsync<bool>("ProcessFiles", new RetryOptions(TimeSpan.FromSeconds(5), 3)
                        {
                            Handle = ex => ex is InvalidOperationException
                        }, fileName);

                        fileProcessTasks.Add(fileProcessTask);
                    }

                    log.LogInformation("Waiting for all file procesing tasks to finish.");
                    var fileProcessTasksResults = await Task.WhenAll(fileProcessTasks);

                    if (Array.TrueForAll(fileProcessTasksResults, value => value))
                    {
                        log.LogInformation($"Number of files processed: {fileProcessTasksResults.Length}");
                    }
                    else
                    {
                        log.LogWarning($"Number of files processed: {fileProcessTasksResults.Length} | Errors: {fileProcessTasksResults.Where(x => !x).Count()}");
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"Caught an error from an activity: {ex}");
                    return new { Success = false, Message = ex };
                }
            }
        }
    }

}