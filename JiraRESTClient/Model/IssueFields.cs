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

        public User Assignee { get; set; }

        public string Description { get; set; }

        public string[] Labels { get; set; }

        public List<Attachment> Attachment { get; set; }

        public Issue Parent { get; set; }

        public List<Issue> Subtasks { get; set; }

        public List<Version> FixVersions { get; set; }
        public List<Version> Versions { get; set; }

    }
}
