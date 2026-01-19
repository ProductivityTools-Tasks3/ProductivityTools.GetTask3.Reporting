using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ProductivityTools.GetTask3.Reporting;

public class Function : IHttpFunction
{
    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(HttpContext context)
    {

        string s = await GetClosedForLast7Days(log);
        // SendEmail(s, log);
        // s = await GetClosedForThisWeek(log);
        // SendEmail(s, log);
        // return new OkObjectResult("Report sent");


        await context.Response.WriteAsync("Hello, Functions Framework.", context.RequestAborted);
    }

    private static async Task<string> GetClosedForLast7Days(ILogger log)
    {
        Action<string> lg = (s) => log.LogInformation(s);
        var rootElement = await new ProductivityTools.GetTask3.Sdk.TaskClient(URL, FirebaseWebApiKey, lg).GetThisWeekFinishedForUser(null, string.Empty, "pwujczyk@gmail.com");
        return await GetClosed(log, rootElement);
    }
}
