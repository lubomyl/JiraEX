using AtlassianConnector.Model.Exceptions;
using ConfluenceEX.Command;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class WorklogViewModel : ViewModelBase
    {
        private RefreshableWorklogViewModel _parent;

        private Issue _issue;

        private IIssueService _issueService;

        private string _timeRemaining;
        private string _timeSpent;
        private string _comment;
        private string _dateStarted;

        public DelegateCommand ConfirmCreateWorklogCommand { get; private set; }
        public DelegateCommand CancelCreateWorklogCommand { get; private set; }

        public WorklogViewModel(RefreshableWorklogViewModel parent, Issue issue, IIssueService issueService)
        {
            this._parent = parent;

            this._issueService = issueService;

            this.Issue = issue;

            this.ConfirmCreateWorklogCommand = new DelegateCommand(ConfirmCreateWorklog);
            this.CancelCreateWorklogCommand = new DelegateCommand(CancelCreateWorklog);
        }

        private void CancelCreateWorklog(object sender)
        {
            (JiraPackage.JiraWorklogToolWindowVar.Frame as IVsWindowFrame).Hide();
        }

        private async void ConfirmCreateWorklog(object sender)
        {
            DateTime parsedDateTime;
            string formattedDate = "";

            string dateStartedNow = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");

            try
            {
                if (this.DateStarted != null)
                {
                    if (DateTime.TryParseExact(this.DateStarted, "yyyy/MM/dd hh:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out parsedDateTime))
                    {
                        formattedDate = parsedDateTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");

                        formattedDate = formattedDate.Remove(formattedDate.Length - 3, 1);
                    }
                    else
                    {
                        //show jiraexception 
                    }
                } else
                {
                    formattedDate = dateStartedNow.Remove(dateStartedNow.Length - 3, 1);
                }

                if (this.TimeSpent != null)
                {
                    await this._issueService.RemarkTimeSpentOnIssue(this.TimeSpent, this.Comment, formattedDate, this._issue.Key);
                }

                if(this.TimeRemaining != null)
                {
                    await this._issueService.RemarkTimeRemainingOnIssue(this.TimeRemaining, this.Issue.Fields.Timetracking.OriginalEstimate, this.Issue.Key);
                }

                this.CancelCreateWorklog(null);
            }
            catch (JiraException ex)
            {
            }
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

        public string TimeSpent
        {
            get
            {
                return this._timeSpent;
            }
            set
            {
                this._timeSpent = value;
                OnPropertyChanged("TimeRemaining");
            }
        }

        public string TimeRemaining
        {
            get { return this._timeRemaining; }
            set
            {
                this._timeRemaining = value;
                OnPropertyChanged("TimeRemaining");
            }
        }

        public string Comment
        {
            get { return this._comment; }
            set
            {
                this._comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public string DateStarted
        {
            get { return this._dateStarted; }
            set
            {
                this._dateStarted = value;
                OnPropertyChanged("DateStarted");
            }
        }
    }
}
