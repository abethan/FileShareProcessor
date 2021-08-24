using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileShareProcessor
{
    public static class TriggerFunction
    {
        [FunctionName(nameof(ProcessFileShareStarter))]
        public static async Task<HttpResponseMessage> ProcessFileShareStarter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            const string instanceId = "FunctionInstance";
            var existingInstance = await starter.GetStatusAsync(instanceId);

            if (existingInstance != null && existingInstance.RuntimeStatus != OrchestrationRuntimeStatus.Completed
                                         && existingInstance.RuntimeStatus != OrchestrationRuntimeStatus.Failed
                                         && existingInstance.RuntimeStatus != OrchestrationRuntimeStatus.Terminated)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("An instance already running."),
                };
            }

            await starter.StartNewAsync("ProcessFileShareOrchestrator", instanceId);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}