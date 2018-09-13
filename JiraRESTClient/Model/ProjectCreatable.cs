using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class ProjectCreatable
    {

        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public List<IssueType> Issuetypes { get; set; }

    }
}
