using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{

    public class CreateIssueViewModel : ViewModelBase, ITitleable
    {

        private JiraToolWindowNavigatorViewModel _parent;
        private BoardProject _project;

        public CreateIssueViewModel(JiraToolWindowNavigatorViewModel parent, BoardProject project)
        {
            this._parent = parent;
            this._project = project;
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", this._project.Name);
        }
    }
}
