using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class PriorityEditableProperty
    {

        public string Key { get; set; }
        public List<Priority> AllowedValues { get; set; }

    }
}
