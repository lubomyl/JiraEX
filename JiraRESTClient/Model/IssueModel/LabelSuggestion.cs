using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class LabelSuggestion
    {

        public string Label { get; set; }
        public bool CheckedStatus { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }
}
