using JiraEX.ViewModel.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class IssueDetailViewModel : ViewModelBase
    {

        private JiraToolWindowNavigatorViewModel _parent;

        public IssueDetailViewModel(JiraToolWindowNavigatorViewModel parent)
        {
            this._parent = parent;
        }

    }
}
