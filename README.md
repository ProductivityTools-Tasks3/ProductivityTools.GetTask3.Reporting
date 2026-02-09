<!--Category:C#--> 
 <p align="right">
    <a href="http://productivitytools.tech/"><img src="Images/Header/ProductivityTools_green_40px_2.png" /><a> 
    <a href="https://github.com/ProductivityTools-Tasks3/ProductivityTools.GetTask3.Contract"><img src="Images/Header/Github_border_40px.png" /></a>
</p>
<p align="center">
    <a href="http://http://productivitytools.tech/">
        <img src="Images/Header/LogoTitle_green_500px.png" />
    </a>
</p>

# GetTask3.Reporting

Azure function which sends report about finished tasks

<!--more-->

Some details

- Function sends report in the MD format every couple hours.
- It uses GetTask3.Sdk nuget packet


## Sent Email
- To sent email ProductivityTools.SentEmailGmail is used
- Password to Gmail is stored in the MasterConfiguration so in the file when debugging locally. When running in azure password is taken from environment variable

## Api Authentication
- Api is protected with the OAuth and Firebase is used as authentication backend
- To perform authentication we need to provide **FirebaseWebApiKey** it is also stored in Master configuration and environment variable

##  Operation not supported
This specific error—NetworkInformationException (95): Operation not supported—is a known issue when running .NET on hardened or serverless Linux environments like Google Cloud Run or Cloud Functions.

The issue occurs because the .NET HttpClient tries to monitor network interface changes (to refresh connection pools) using low-level sockets that are restricted in the Cloud Run sandbox.
To resolve it we can add env variables or add code to the function:
```
static Function()
{
    Environment.SetEnvironmentVariable("DOTNET_NetworkChange_UNSUPPORTED", "true");
    Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT", "false");
}
```

### Google Functions

```
dotnet new gcf-http
dotnet add package ProductivityTools.GetTask3.Sdk
dotnet add package ProductivityTools.MasterConfiguration
```

## Secret Manager


![](2026-01-21-21-06-19.png)