using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class IssueFields
    {

        public IssueType Issuetype { get; set; }

        public Sprint Sprint { get; set; }
        public Project Project { get; set; }

        public string Summary { get; set; }
        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}
