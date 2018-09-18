using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class Sprint
    {

        public Sprint(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }

    }
}
