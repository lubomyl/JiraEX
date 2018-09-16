using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class FixVersion
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public bool Archived { get; set; }
        public bool Released { get; set; }
        public string ReleaseDate { get; set; }

    }
}
