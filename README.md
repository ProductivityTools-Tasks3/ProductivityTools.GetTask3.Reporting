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
