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

        public const int JIRA_TOOLBAR_ID = 0x1000;
        public const int JIRA_COMMAND_ID = 0x0101;

        public static readonly Guid guidJiraCommand = new Guid(GUID_JIRA_COMMAND_STRING);
        public static readonly Guid guidJiraPackage = new Guid(GUID_JIRA_PACKAGE_STRING);

    }
}
