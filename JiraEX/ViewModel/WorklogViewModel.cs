using AtlassianConnector.Model;
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

        private IssueDetailViewModel _refreshViewModel;

        private string _timeRemaining;
        private string _timeSpent;
        private string _comment;
        private string _dateStarted;
        private string _originalEstimate;
        private string _remainingEstimateTitle;
        private string _timeSpentTitle;

        public DelegateCommand ConfirmCreateWorklogCommand { get; private set; }
        public DelegateCommand CancelCreateWorklogCommand { get; private set; }
        public DelegateCommand SetDateStartedToNowCommand { get; private set; }

        public WorklogViewModel(RefreshableWorklogViewModel parent, Issue issue, IIssueService issueService, IssueDetailViewModel refreshViewModel)
        {
            this._parent = parent;

            HideErrorMessages(this._parent);

            this._issueService = issueService;

            this._refreshViewModel = refreshViewModel;

            this.Issue = issue;
            InitializeTitleTexts();

            this.ConfirmCreateWorklogCommand = new DelegateCommand(ConfirmCreateWorklog);
            this.CancelCreateWorklogCommand = new DelegateCommand(CancelCreateWorklog);
            this.SetDateStartedToNowCommand = new DelegateCommand(SetDateStartedToNow);
        }

        private void InitializeTitleTexts()
        {
            if (this.Issue.Fields.Timetracking.OriginalEstimate == null)
            {
                OriginalEstimate = "0m";
            }
            else
            {
                OriginalEstimate = this.Issue.Fields.Timetracking.OriginalEstimate;
            }

            if (this.Issue.Fields.Timetracking.RemainingEstimate == null)
            {
                RemainingEstimateTitle = "0m";
            }
            else
            {
                RemainingEstimateTitle = this.Issue.Fields.Timetracking.RemainingEstimate;
            }

            if (this.Issue.Fields.Timetracking.TimeSpent == null)
            {
                TimeSpentTitle = "0m";
            }
            else
            {
                TimeSpentTitle = this.Issue.Fields.Timetracking.TimeSpent;
            }
        }

        private void SetDateStartedToNow(object obj)
        {
            this.DateStarted = DateTime.UtcNow.ToString("yyyy'/'MM'/'dd HH:mm"); 
        }

        private void CancelCreateWorklog(object sender)
        {
            (JiraPackage.JiraWorklogToolWindowVar.Frame as IVsWindowFrame).Hide();
        }

        private async void ConfirmCreateWorklog(object sender)
        {
            this._parent.StartLoading();

            bool anyTaskFired = false;

            DateTime parsedDateTime;
            string formattedDate = "";

            string dateStartedNow = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");

            try
            {
                if (this.DateStarted != null && !this.DateStarted.Equals(""))
                {
                    if (DateTime.TryParseExact(this.DateStarted, "yyyy/MM/dd HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out parsedDateTime))
                    {
                        formattedDate = parsedDateTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");

                        formattedDate = formattedDate.Remove(formattedDate.Length - 3, 1);
                    }
                    else
                    {
                        ErrorResponse er = new ErrorResponse();
                        er.ErrorMessages = new string[1];
                        er.ErrorMessages[0] = "Invalid date started format.";

                        throw new JiraException(er);
                    }
                }
                else
                {
                    formattedDate = dateStartedNow.Remove(dateStartedNow.Length - 3, 1);
                }

                if ((this.TimeSpent != null && !this.TimeSpent.Equals("")) || (this.Comment != null && !this.Comment.Equals("")))
                {
                    anyTaskFired = true;
                    await this._issueService.RemarkTimeSpentOnIssue(this.TimeSpent, this.Comment, formattedDate, this._issue.Key);
                }

                if(this.TimeRemaining != null && !this.TimeRemaining.Equals(""))
                {
                    anyTaskFired = true;
                    await this._issueService.RemarkTimeRemainingOnIssue(this.TimeRemaining, this.Issue.Fields.Timetracking.OriginalEstimate, this.Issue.Key);
                }

                this._refreshViewModel.UpdateIssueAsync();

                if (anyTaskFired)
                {
                    this.CancelCreateWorklog(null);
                }

                this._parent.StopLoading();
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
                this._parent.StopLoading();
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
                OnPropertyChanged("TimeSpent");
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

        public string OriginalEstimate
        {
            get
            {
                return this._originalEstimate;
            }
            set
            {
                this._originalEstimate = value;
                OnPropertyChanged("OriginalEstimate");
            }
        }

        public string RemainingEstimateTitle
        {
            get
            {
                return this._remainingEstimateTitle;
            }
            set
            {
                this._remainingEstimateTitle = value;
                OnPropertyChanged("RemainingEstimateTitle");
            }
        }

        public string TimeSpentTitle
        {
            get
            {
                return this._timeSpentTitle;
            }
            set
            {
                this._timeSpentTitle = value;
                OnPropertyChanged("TimeSpentTitle");
            }
        }
    }
}
