using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class IssueLinkType
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Inward { get; set; }
        public string Outward { get; set; }

    }
}
