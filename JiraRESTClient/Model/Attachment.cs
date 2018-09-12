using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class Attachment
    {

        public string Id { get; set; }
        public string Filename { get; set; }
        public DateTime Created { get; set; }
        public double Size { get; set; }
        public string Content { get; set; }
    }
}
