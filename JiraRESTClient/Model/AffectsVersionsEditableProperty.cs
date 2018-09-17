using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class AffectsVersionsEditableProperty
    {

        public string Key { get; set; }

        public List<Version> AllowedValues { get; set; }

    }
}
