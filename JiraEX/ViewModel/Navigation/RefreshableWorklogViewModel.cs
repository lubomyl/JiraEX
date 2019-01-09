using JiraEX.Main;
using JiraEX.View;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel.Navigation
{

    public class RefreshableWorklogViewModel : ViewModelBase
    {

        private JiraWorklogToolWindow _parent;

        private string _errorMessage;

        private bool _isLoading;

        private WorklogView _selectedView;

        public RefreshableWorklogViewModel(JiraWorklogToolWindow parent)
        {
            this._parent = parent;
        }

        public void OpenCreateWorklogForIssue(Issue issue, IIssueService issueService, IssueDetailViewModel refreshViewModel)
        {
            this._parent.Caption = issue.Key;
            this.SelectedView = new WorklogView(this, issue, issueService, refreshViewModel);
        }

        public WorklogView SelectedView
        {
            get { return this._selectedView; }
            set
            {
                this._selectedView = value;
                OnPropertyChanged("SelectedView");
            }
        }

        public void StartLoading()
        {
            this.IsLoading = true;
        }

        public void StopLoading()
        {
            this.IsLoading = false;
        }

        public void SetErrorMessage(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public bool IsLoading
        {
            get { return this._isLoading; }
            set
            {
                this._isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public string ErrorMessage
        {
            get { return this._errorMessage; }
            set
            {
                this._errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

    }
}
