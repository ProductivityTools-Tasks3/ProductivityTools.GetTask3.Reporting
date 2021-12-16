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



        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await Work(log);

            return new OkObjectResult("Fda");

        }

        private static string FindClosed(string path, Contract.ElementView element)
        {
            if (element.Finished.HasValue && element.Finished.Value > DateTime.Now.AddDays(-1))
            {
                return string.Concat(path, element.Name) + Environment.NewLine;
            }
            var r = string.Empty;
            foreach (var item in element.Elements)
            {
                r += FindClosed(element.Name, item);
            }
            return r;
        }

        private static async Task Work(ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            Action<string> lg = (s) => log.LogInformation(s);
            var rootElement = await ProductivityTools.GetTask3.Sdk.TaskClient.GetStructure(null, string.Empty, lg);
            string resut = FindClosed(rootElement.Name, rootElement);
        }

    }
}
