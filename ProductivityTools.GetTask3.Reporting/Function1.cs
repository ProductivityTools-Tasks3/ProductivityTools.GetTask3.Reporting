using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ProductivityTools.GetTask3.Reporting
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            await Work(log);

        }

          

        [FunctionName("Function4")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await Work(log);

            return new OkObjectResult("Fda");

        }

        private static async Task Work(ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            Action<string> lg = (s) => log.LogInformation(s);
            var rootElement = await ProductivityTools.GetTask3.Sdk.TaskClient.GetStructure(null, string.Empty, lg);
        }

    }
}
