using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductivityTools.MasterConfiguration;

namespace ProductivityTools.GetTask3.Reporting
{
    public static class Function1
    {

        static string URL = "https://apigettask3.productivitytools.tech:8040/api/";// Consts.EndpointAddress;
        //static string URL = "http://localhost:5513/api/";// Consts.EndpointAddress;

        [FunctionName("SendReport")]
        public static async Task Run([TimerTrigger("0 */5 * * *")] TimerInfo myTimer, ILogger log)
        {
            string s = await GetClosed(log);
            SendEmail(s, log);
        }



        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string s = await GetClosed(log);
            SendEmail(s, log);
            return new OkObjectResult("Fda");

        }

        [FunctionName("GetDateTime")]
        public static string GetDateTime(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
          ILogger log)
        {

            log.LogInformation(System.Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            return DateTime.Now.ToString();

        }


        private static string FindClosed(string path, Contract.ElementView element)
        {
            if (element.Finished.HasValue && element.Finished.Value > DateTime.Now.AddDays(-1))
            {
                return string.Concat(path, "\\", element.Name) + Environment.NewLine;
            }
            var r = string.Empty;
            foreach (var item in element.Elements)
            {
                r += FindClosed(element.Name, item);
            }
            return r;
        }

        private static IConfigurationRoot Configuration
        {
            get
            {
                var configuration = new ConfigurationBuilder()
                       .AddMasterConfiguration("ProductivityTools.GetTask3.Client.json")
                       .AddEnvironmentVariables()
                       .Build();
                return configuration;
            }
        }

        private static string FirebaseWebApiKey
        {
            get
            {
                var key = Configuration["FirebaseWebApiKey"];
                return key;
            }
        }


        private static async Task<string> GetClosed(ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            Action<string> lg = (s) => log.LogInformation(s);
            var rootElement = await new ProductivityTools.GetTask3.Sdk.TaskClient(URL, FirebaseWebApiKey, lg).GetStructure(null, string.Empty);
            string result = FindClosed(rootElement.Name, rootElement);
            return result;
        }

        private static void SendEmail(string body,ILogger log)
        {
            string password = Configuration["GmailPassword"];
            log.LogInformation("gmail pass");
            log.LogInformation(password);
            SendEmailGmail.Gmail.Send("productivitytools.tech@gmail.com", password, "pwujczyk@hotmail.com", "GetTask3", body);
        }

    }
}
