using JiraEX.ViewModel.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class NoIssueFoundViewModel : ViewModelBase, ITitleable
    {

        private IJiraToolWindowNavigatorViewModel _parent;

        private string _issueKey;

        public NoIssueFoundViewModel(IJiraToolWindowNavigatorViewModel parent, string issueKey)
        {
            this._parent = parent;

            this._issueKey = issueKey;

            SetPanelTitles();
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "No issue found");
        }

        public string IssueKey
        {
            get { return this._issueKey; }
        }
    }
}
