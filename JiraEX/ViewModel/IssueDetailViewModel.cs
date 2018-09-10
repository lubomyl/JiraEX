using ConfluenceEX.Command;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class IssueDetailViewModel : ViewModelBase
    {
        private bool _isEditingSummary = false;

        private JiraToolWindowNavigatorViewModel _parent;

        private IIssueService _issueService;

        private Issue _issue;

        public DelegateCommand EditSummaryCommand { get; private set; }
        public DelegateCommand ConfirmEditSummaryCommand { get; private set; }
        public DelegateCommand CancelEditSummaryCommand { get; private set; }

        public IssueDetailViewModel(JiraToolWindowNavigatorViewModel parent, Issue issue)
        {
            this._parent = parent;

            this._issueService = new IssueService();

            this._issue = issue;

            this.EditSummaryCommand = new DelegateCommand(EnableEditSummary);
            this.ConfirmEditSummaryCommand = new DelegateCommand(ConfirmEditSummary);
            this.CancelEditSummaryCommand = new DelegateCommand(CancelEditSummary);
        }

        private void EnableEditSummary(object parameter)
        {
            this.IsEditingSummary = true;
        }

        private void ConfirmEditSummary(object parameter)
        {
            this._issueService.UpdateIssueSummaryAsync(this._issue.Key, this._issue.Fields.Summary);

            this.IsEditingSummary = false;
        }

        private void CancelEditSummary(object parameter)
        {
            this.IsEditingSummary = false;
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

        public bool IsEditingSummary
        {
            get { return this._isEditingSummary; }
            set
            {
                this._isEditingSummary = value;
                OnPropertyChanged("IsEditingSummary");
            }
        }

    }
}
