using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class EditablePropertiesFields
    {

        public PriorityEditableProperty Priority { get; set; }
        public AffectsVersionsEditableProperty Versions { get; set; }
        public FixVersionsEditableProperty FixVersions { get; set; }

    }
}
