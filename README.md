# JiraEX

Visual Studio extension integrating Atlassian Jira.

##### Testing with own Jira instance
See: [Wiki - How to configure ConfluenceEX as consumer](https://github.com/lubomyl/ConfluenceEX/wiki/How-to-configure-ConfluenceEX-as-consumer)

_Works same for Jira_

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

##### Authentication protection
Need to sign-in at [My testing Atlassian server](https://lubomyl4.atlassian.net)  

##### What does it demonstrate?
REST Client integrated into ToolWindow  
After project start-up goto and click on `View|Jira` if the Jira Toolbar is not already shown    
New ToolWindow with ToolBar will show up  
  
Before successful sign-in:  
- If not already signed-in with oauth_access_token from registry User Settings Store click on `Redirect`
- By Signing-in and clicking on `Allow/Přijmout` give JiraEX rights to make rest api calls with your identity
- Copy generated oauth_verifier (ověřovací kód) and navigate back to JiraEX
- Paste copied oauth oauth_verifier (ověřovací kód) and click on `Sign-in`

After successful sign-in:  
- On ToolBar click on `Connect` to check authentication state
- On ToolBar click on `Home` icon to show list of available Projects
- Double-click on specific `Project` to see list of Issues available under this Project
- Click on specific `Issue` to see details
- On Issue view edit property by clicking on its value

## Solution structure
- JiraEX *(main)*
- JiraRESTClient *(class library)*
- AtlassianConnector *(class library)* available at: [AtlassianConnector](https://github.com/lubomyl/AtlassianConnector)

## Dependencies
- RestSharp 106.2.1: currently not in use *(no support for OAuth with RSA)*
- DevDefined.OAuth 0.2.0
