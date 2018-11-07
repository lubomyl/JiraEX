using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class IssueListPaged
    {

        public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }

        public List<Issue> Issues { get; set; }
    }
}
