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
        Action<string> consoleLog = (s) => Console.WriteLine(s);
        var Log = consoleLog;
        consoleLog("===== Function started =====");

        //string s = await GetClosedForLast7Days(consoleLog);
        // SendEmail(s, log);
        string s = await GetClosedForThisWeek(consoleLog);
        SendEmail(s, consoleLog);

        var result = "Hello, The report:" + Environment.NewLine;
        result += s;
        result += Environment.NewLine;
        result += "The end!";
        //await context.Response.WriteAsync(s, context.RequestAborted);
        await context.Response.WriteAsync("Report sent!", context.RequestAborted);
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


        var inbox = await FindElements(rootElement, new List<string> { "PawelPC", "NetSys" });
        string result = string.Empty;
        foreach (var i in inbox)
        {
            result += await GetPathToRoot(rootElement, i);
            result += Environment.NewLine;
            //result += ReportMd.PrepareReport(i);
            result += ReportSimple.PrepareReport(i);
        }
        return result;
    }

    private static void SendEmail(string body, Action<string> log)
    {
        string password = Configuration["GmailPassword"];
        log("gmail pass");
        log(password);
        SendEmailGmail.Gmail.Send("productivitytools.tech@gmail.com", password, "pwujczyk@gmail.com", "GetTask3", body);
    }

}
