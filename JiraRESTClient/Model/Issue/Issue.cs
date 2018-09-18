using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class Issue
    {

        public int Id { get; set; }
        public string Key { get; set; }

        public IssueFields Fields { get; set; }

    }
}
