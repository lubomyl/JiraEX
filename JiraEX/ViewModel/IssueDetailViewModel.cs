using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
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

        private Issue _issue;

        public IssueDetailViewModel(JiraToolWindowNavigatorViewModel parent, Issue issue)
        {
            this._parent = parent;

            this._issue = issue;
        }

        public Issue Issue
        {
            get { return this._issue; }
            set
            {
                this._issue = value;
                OnPropertyChanged("Issue");
            }
        }

    }
}
