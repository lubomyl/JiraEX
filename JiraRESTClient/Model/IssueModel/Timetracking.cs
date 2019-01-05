using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model.IssueModel
{
    public class Timetracking
    {

        public string OriginalEstimate { get; set; }
        public string RemainingEstimate { get; set; }
        public string TimeSpent { get; set; }

        public double OriginalEstimateSeconds { get; set; }
        public double RemainingEstimateSeconds { get; set; }
        public double TimeSpentSeconds { get; set; }
    }
}
