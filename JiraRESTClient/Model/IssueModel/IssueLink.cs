using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class IssueLink
    {

        public string Id { get; set; }
        public IssueLinkType Type { get; set; }

        public Issue InwardIssue { get; set; }
        public Issue OutwardIssue { get; set; }

    }
}
