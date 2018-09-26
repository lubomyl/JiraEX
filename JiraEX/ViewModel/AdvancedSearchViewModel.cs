using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class AdvancedSearchViewModel : ViewModelBase, ITitleable
    {

        private IJiraToolWindowNavigatorViewModel _parent;

        public AdvancedSearchViewModel(JiraToolWindowNavigatorViewModel parent)
        {
            this._parent = parent;
        }

        public void OnItemSelected(Issue issue)
        {
            this._parent.ShowIssueDetail(issue, null);
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Advanced search");
        }
    }
}
