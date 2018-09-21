using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class Filter
    {

        string Id { get; set; }
        string Name { get; set; }
        string Jql { get; set; }
    }
}
