using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class MyAttribute
    {

        public MyAttribute(string name, bool checkedStatus)
        {
            this.Name = name;
            this.CheckedStatus = checkedStatus;
        }

        public MyAttribute()
        {

        }

        public string Name { get; set; }
        public bool CheckedStatus { get; set; }

    }
}
