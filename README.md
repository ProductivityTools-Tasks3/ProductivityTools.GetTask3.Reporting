<table>
  <tr>
    <td>
      <a href="https://github.com/ProductivityTools-Tasks3/ProductivityTools.GetTask3.Reporting"><img src="Images/Header/ProductivityTools_green_40px_2.png" alt="ProductivityTools logo" /></a>
    </td>
    <td>
      <a href="https://github.com/ProductivityTools-Tasks3/ProductivityTools.GetTask3.Reporting">
        <img src="Images/Header/LogoTitle_green_500px.png" alt="ProductivityTools.GetTask3.Reporting title" />
      </a>
    </td>
    <td align="right">
      <a href="https://github.com/ProductivityTools-Tasks3/ProductivityTools.GetTask3.Reporting"><img src="Images/Header/Github_border_40px.png" alt="Go to repository" /></a>
      <a href="https://www.nuget.org/packages/ProductivityTools.GetTask3.Reporting/"><img src="Images/Header/Nuget_border_40px.png" alt="Go to nuget package" /></a>
    </td>
  </tr>
</table>

# ProductivityTools.GetTask3.Reporting

[![Nuget](https://img.shields.io/nuget/v/ProductivityTools.GetTask3.Reporting.svg)](https://www.nuget.org/packages/ProductivityTools.GetTask3.Reporting/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This library generates a report from the GetTask3 application and can optionally send it via email.

## Installation

Install the package via NuGet Package Manager:

```powershell
Install-Package ProductivityTools.GetTask3.Reporting
```

Or via the .NET CLI:

```shell
dotnet add package ProductivityTools.GetTask3.Reporting
```

## Usage

The main component of the library is the `ReportGenerator` class. To use it, you need to provide the GetTask3 API URL, a Firebase Web API Key, and a Gmail password for the sending account.

```csharp
using ProductivityTools.GetTask3.Reporting;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Replace with your actual credentials and URL
        string url = "https://your-gettask3-api-url.com";
        string firebaseWebApiKey = "your_firebase_web_api_key";
        string gmailPassword = "your_gmail_password";

        // Create an instance of the report generator
        var reportGenerator = new ReportGenerator(url, firebaseWebApiKey, gmailPassword);

        // Define a logger action (e.g., writing to the console)
        Action<string> consoleLog = (message) => Console.WriteLine(message);

        // Generate the report. Set sendReport to true to email it.
        // Note: The recipient is currently configured within the library logic.
        bool sendReport = false;
        string reportContent = await reportGenerator.GenerateReport(consoleLog, sendReport);

        Console.WriteLine("\n--- Generated Report ---");
        Console.WriteLine(reportContent);
    }
}
```

### Key Components

*   **`ReportGenerator(string url, string firebaseWebApiKey, string gmailPassword)`**: The constructor requires the API endpoint, Firebase key, and the password for the Gmail account used for sending emails.
*   **`GenerateReport(Action<string> consoleLog, bool sendReport)`**: This asynchronous method fetches the data, generates the report string, and, if `sendReport` is `true`, emails the report. It takes a logger action to provide status updates.

## Contributing

Contributions are welcome. Please open an issue or submit a pull request on the [GitHub repository](https://github.com/ProductivityTools-Tasks3/ProductivityTools.GetTask3.Reporting).

## License

This project is licensed under the MIT License. See the [LICENSE](https://opensource.org/licenses/MIT) file for details.
