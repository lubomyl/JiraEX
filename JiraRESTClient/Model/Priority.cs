using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class Priority
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        public string Name { get; set; }

    }
}
