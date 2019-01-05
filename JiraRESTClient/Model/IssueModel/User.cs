using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class User
    {

        public User()
        {
        }

        public User(string displayName, string name)
        {
            this.DisplayName = displayName;
            this.Name = name;
        }

        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
