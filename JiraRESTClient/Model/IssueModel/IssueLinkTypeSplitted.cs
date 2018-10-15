using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class IssueLinkTypeSplitted
    {

        public IssueLinkTypeSplitted(string id, string name, string relationName, string type)
        {
            this.Id = id;
            this.Name = name;
            this.RelationName = relationName;
            this.Type = type;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string RelationName { get; set; }
        public string Type { get; set; }

    }
}
