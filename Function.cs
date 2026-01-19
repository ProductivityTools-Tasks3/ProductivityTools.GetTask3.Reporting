using System;
using System.Collections.Generic;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ProductivityTools.MasterConfiguration;

namespace ProductivityTools.GetTask3.Reporting;

public class Function : IHttpFunction
{
    static string URL = "https://apigettask3.productivitytools.top:8042/api/";// Consts.EndpointAddress;

    public async Task HandleAsync(HttpContext context)
    {

        Action<string> consoleLog = (s) => Console.WriteLine(s);

        string s = await GetClosedForLast7Days(consoleLog);
        // SendEmail(s, log);
        // s = await GetClosedForThisWeek(log);
        // SendEmail(s, log);
        // return new OkObjectResult("Report sent");


        await context.Response.WriteAsync("Hello, Functions Framework.", context.RequestAborted);
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
        var rootElement = await new ProductivityTools.GetTask3.Sdk.TaskClient(URL, FirebaseWebApiKey, log).GetThisWeekFinishedForUser(null, string.Empty, "pwujczyk@gmail.com");
        return await GetClosed(log, rootElement);
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
