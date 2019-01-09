# JiraEX

Visual Studio extension integrating Atlassian Jira.

##### Testing with own Jira instance
See: [Wiki - How to setup OAuth (Jira administrator)](https://github.com/lubomyl/JiraEX/wiki/How-to-setup-OAuth-(Jira-administrator))

##### Missing ignored settings file
See: [Wiki - Ignored setting file](https://github.com/lubomyl/ConfluenceEX/wiki/Ignored-settings-file)

_Works same for Jira_

##### How to debug
- Mark `JiraEX` as StartUp Project  
- Set Debug mode  
- In order to load project as VSPackage in Experimental instance of VS follow [CodeProject](https://www.codeproject.com/Tips/832362/Resetting-the-Visual-Studio-Experimental-Instance) or do following:  
  1. Right-click on `ConfluenceEX project|Properties`
  2. Select `Debug` bookmark
  3. As external program navigate to: `"MicrosoftVS installation location"\Common7\IDE\devenv.exe`
  4. As command line argument on  start-up add `/RootSuffix Exp`
- Build&Start  

## Solution structure
- JiraEX *(main)*
- JiraRESTClient *(class library)*
- AtlassianConnector *(class library)* available at: [AtlassianConnector](https://github.com/lubomyl/AtlassianConnector)

## Dependencies
- RestSharp 106.2.1
- DevDefined.OAuth 0.2.0
