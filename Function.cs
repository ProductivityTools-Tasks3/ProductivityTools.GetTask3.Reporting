using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductivityTools.MasterConfiguration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductivityTools.GetTask3.Reporting;

public class Function : IHttpFunction
{

    static string URL = "https://tasks-api.productivitytools.top/api/";// Consts.EndpointAddress;
    //static string URL = "http://localhost:5513/api/";// Consts.EndpointAddress;
    
    //this is required to perform post operation from the cloud run environment
    static Function()
    {
        Environment.SetEnvironmentVariable("DOTNET_NetworkChange_UNSUPPORTED", "true");
        Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT", "false");
    }

    public async Task HandleAsync(HttpContext context)
    {
        //in Launch settings this env variable is set
        // Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\GitHub\Home.Configuration\ptprojectsweb-firebase-adminsdk.json");
        
        //x Environment.SetEnvironmentVariable("DOTNET_NetworkChange_UNSUPPORTED", "true");
        Action<string> consoleLog = (s) => Console.WriteLine(s);
        var Log = consoleLog;
        consoleLog("===== Function started =====");
        
        //x
        //using var handler = new HttpClientHandler();
        //using var httpClient = new HttpClient(handler);
        //HttpResponseMessage testresponse = await httpClient.GetAsync("http://www.wp.pl");
        //Log("[GetIdToken] test get response suceed");

        //var testData = new
        //{
        //    title = "Testowy Post",
        //    body = "To jest treœæ wys³ana z mojej aplikacji",
        //    userId = 1
        //};

        //string json = Newtonsoft.Json.JsonConvert.SerializeObject(testData);
        //var content1 = new StringContent(json, Encoding.UTF8, "application/json");

        //string TestUrl = "https://jsonplaceholder.typicode.com/posts";
        //Log("[GetIdToken] test post response try");
        //HttpResponseMessage testresponsepost = await httpClient.PostAsync(TestUrl, content1);
        //Log("[GetIdToken] test post response suceed");
        //var responseContent1 = await testresponsepost.Content.ReadAsStringAsync();
        //Log("[GetIdToken] test post response content:" + responseContent1);
        //x

        string s = await GetClosedForLast7Days(consoleLog);
        // SendEmail(s, log);
        // s = await GetClosedForThisWeek(log);
        // SendEmail(s, log);
        // return new OkObjectResult("Report sent");


        await context.Response.WriteAsync("Hello, Functions Framework." + s + "pawel", context.RequestAborted);
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

    private static async Task<List<Contract.ElementView>> FindElements(Contract.ElementView root, List<string> names)
    {
        var result = new List<Contract.ElementView>();
        foreach (var item in names)
        {
            var r1 = await FindElement(root, item);
            result.AddRange(r1);
        }
        return result;
    }

    private static async Task<List<Contract.ElementView>> FindElement(Contract.ElementView root, string name)
    {
        var result = new List<Contract.ElementView>();
        if (root.Name == name)
        {
            result.Add(root);
        }
        else
        {
            foreach (var el in root.Elements)
            {

                var temp = await FindElement(el, name);

                result.AddRange(temp);

            }
        }
        return result;
    }

    private static async Task<Contract.ElementView> FindElement(Contract.ElementView root, int id)
    {
        if (root.ElementId == id)
        {
            return root;
        }
        else
        {
            foreach (var el in root.Elements)
            {

                var temp = await FindElement(el, id);
                if (temp != null)
                {
                    return temp;
                }
            }
        }
        return null;
    }

    private static async Task<string> GetPathToRoot(Contract.ElementView root, Contract.ElementView element)
    {
        string s = string.Empty;
        while (element != null && element.ParentId != null)
        {
            s = s.Insert(0, element.Name + " >>");
            element = await FindElement(root, element.ParentId.Value);
        }
        return s;
    }

    private static async Task<string> GetClosedForThisWeek(Action<string> log)
    {
        var rootElement = await new ProductivityTools.GetTask3.Sdk.TaskClient(URL, FirebaseWebApiKey, log).GetThisWeekFinishedForUser(null, string.Empty, "pwujczyk@gmail.com");
        return await GetClosed(log, rootElement);
    }

    private static async Task<string> GetClosedForLast7Days(Action<string> log)
    {
        log("Get Closed for Last 7 days");
        try
        {
            var rootElement = await new ProductivityTools.GetTask3.Sdk.TaskClient(URL, FirebaseWebApiKey, log).GetThisWeekFinishedForUser(null, string.Empty, "pwujczyk@gmail.com");
            return await GetClosed(log, rootElement);
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return string.Empty;

    }

    private static async Task<string> GetClosed(Action<string> log, Contract.ElementView rootElement)
    {
        log($"C# Timer trigger function executed at: {DateTime.Now}");

        log("firebase weba pi key");
        log(FirebaseWebApiKey);


        var inbox = await FindElements(rootElement, new List<string> { "PawelPC", "Google" });
        string result = string.Empty;
        foreach (var i in inbox)
        {
            result += await GetPathToRoot(rootElement, i);
            //result += ReportMd.PrepareReport(i);
        }
        return result;
    }

    private static void SendEmail(string body, Action<string> log)
    {
        string password = Configuration["GmailPassword"];
        log("gmail pass");
        log(password);
        //SendEmailGmail.Gmail.Send("productivitytools.tech@gmail.com", password, "pwujczyk@gmail.com", "GetTask3", body);
    }

}
