using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductivityTools.GetTask3.Reporting;
public class ReportGenerator
{

    private string URL;
    private string FirebaseWebApiKey;
    private string GmailPassword;
    public ReportGenerator(string url, string firebaseWebApiKey, string gmailPassword   )
    {
        this.URL = url;
        this.FirebaseWebApiKey = firebaseWebApiKey;
        this.GmailPassword = gmailPassword;
    }

    public  async Task<string> GenerateReport(Action<string> consoleLog, bool sendReport)
    {
        string s = await GetClosedForThisWeek(consoleLog);
        if (sendReport)
        {
            SendEmail(s, consoleLog);
        }
        return s;
    }

    private  async Task<string> GetClosedForThisWeek(Action<string> log)
    {
        var rootElement = await new ProductivityTools.GetTask3.Sdk.TaskClient(URL, FirebaseWebApiKey, log).GetThisWeekFinishedForUser(null, string.Empty, "pwujczyk@gmail.com");
        return await GetClosed(log, rootElement);
    }

    private async Task<string> GetClosedForLast7Days(Action<string> log)
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

    private  async Task<string> GetClosed(Action<string> log, Contract.ElementView rootElement)
    {
        log($"C# Timer trigger function executed at: {DateTime.Now}");

        log("firebase weba pi key");
        log(FirebaseWebApiKey);


        var inbox = await FindElements(rootElement, new List<string> { "PawelPC", "NetSys" });
        string result = string.Empty;
        foreach (var i in inbox)
        {
            result += await GetPathToRoot(rootElement, i);
            result += "<br/>";
            //result += ReportMd.PrepareReport(i);
            result += ReportSimple.PrepareReport(i);
        }
        return result;
    }

    private async Task<string> GetPathToRoot(Contract.ElementView root, Contract.ElementView element)
    {
        string s = string.Empty;
        while (element != null && element.ParentId != null)
        {
            s = s.Insert(0, element.Name + " >>");
            element = await FindElement(root, element.ParentId.Value);
        }
        return s;
    }


    private async Task<List<Contract.ElementView>> FindElements(Contract.ElementView root, List<string> names)
    {
        var result = new List<Contract.ElementView>();
        foreach (var item in names)
        {
            var r1 = await FindElement(root, item);
            result.AddRange(r1);
        }
        return result;
    }

    private  async Task<List<Contract.ElementView>> FindElement(Contract.ElementView root, string name)
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

    private  async Task<Contract.ElementView> FindElement(Contract.ElementView root, int id)
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

    private  void SendEmail(string body, Action<string> log)
    {
        string password = GmailPassword;
        log("gmail pass");
        log(password);
        SendEmailGmail.Gmail.Send("productivitytools.tech@gmail.com", password, "pwujczyk@gmail.com", "GetTask3", body);
    }

}