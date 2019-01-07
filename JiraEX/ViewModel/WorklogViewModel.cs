using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class WorklogViewModel : ViewModelBase
    {
        private Issue _issue;
        private int _randomInt;

        public WorklogViewModel(Issue issue)
        {
            this.Issue = issue;
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
