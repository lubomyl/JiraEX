using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.Main
{
    static class Guids
    {

        public const string GUID_JIRA_COMMAND_STRING = "3F01A173-9B5A-4CCF-A484-EB41152ABAB2";
        public const string GUID_JIRA_PACKAGE_STRING = "923D3D57-D913-4751-93AA-14D5026B1144";
        public const string GUID_JIRA_TOOL_WINDOW_STRING = "7953b9ea-8958-4cea-abe9-7cf58fc7d0c6";
        public const string GUID_JIRA_TOOLBAR_MENU_STRING = "81EE54E9-FC51-4A19-8964-6C52F1917B97";

        public const int COMMAND_HOME_ID = 0x0129;
        public const int COMMAND_CONNECTION_ID = 0x0132;
        public const int COMMAND_FILTERS_ID = 0x0134;

        public const int JIRA_TOOLBAR_ID = 0x1000;
        public const int JIRA_COMMAND_ID = 0x0101;

        public static readonly Guid guidJiraCommand = new Guid(GUID_JIRA_COMMAND_STRING);
        public static readonly Guid guidJiraPackage = new Guid(GUID_JIRA_PACKAGE_STRING);
        public static readonly Guid guidJiraToolbarMenu = new Guid(GUID_JIRA_TOOLBAR_MENU_STRING);
    }
}
