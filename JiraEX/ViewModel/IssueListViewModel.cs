using ConfluenceEX.Command;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JiraEX.ViewModel
{
    public class IssueListViewModel : ViewModelBase, ITitleable
    {
        private IIssueService _issueService;
        private ISprintService _sprintService;

        private IJiraToolWindowNavigatorViewModel _parent;

        private BoardProject _boardProject;

        private ObservableCollection<Issue> _issueList;
        private ObservableCollection<Sprint> _sprintList;

        private Sprint _selectedSprint;
        private Sprint _defaultSprintSelected;

        public DelegateCommand CreateIssueCommand { get; private set; }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, BoardProject boardProject)
        {
            this._issueService = new IssueService();
            this._sprintService = new SprintService();

            this._parent = parent;

            this._boardProject = boardProject;

            this.IssueList = new ObservableCollection<Issue>();
            this.SprintList = new ObservableCollection<Sprint>();

            this.CreateIssueCommand = new DelegateCommand(RedirectCreateIssue);

            this._defaultSprintSelected = new Sprint("0", "All sprints");

            OleMenuCommandService service = JiraPackage.Mcs;

            GetIssuesAsync();
            GetSprintsAsync();

            this.IssueList.CollectionChanged += this.OnCollectionChanged;
            this.SprintList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }

        private void RedirectCreateIssue(object sender)
        {
            this._parent.CreateIssue(this._boardProject);
        }

        private async void GetIssuesAsync()
        {
            Task<IssueList> issueTask = this._issueService.GetAllIssuesOfBoardAsync(this._boardProject.Id);

            var issueList = await issueTask as IssueList;

            this.IssueList.Clear();

            foreach (Issue i in issueList.Issues)
            {
                this.IssueList.Add(i);
            }
        }

        private void GetIssuesBySprintAsync()
        {
            Task<IssueList> issueTask = this._issueService.GetAllIssuesOfBoardOfSprintAsync(this._boardProject.Id, this._selectedSprint.Id);

            var issueList = issueTask.Result as IssueList;

            this.IssueList.Clear();

            foreach (Issue i in issueList.Issues)
            {
                this.IssueList.Add(i);
            }
        }

        private void GetSprintsAsync()
        {
            Task<SprintList> sprintTask = this._sprintService.GetAllSprintsOfBoardtAsync(this._boardProject.Id);

            var sprintList = sprintTask.Result as SprintList;

            this.SprintList.Add(this._defaultSprintSelected);
            this.SelectedSprint = this.SprintList[0];

            foreach (Sprint s in sprintList.Values)
            {
                this.SprintList.Add(s);
            }
        }

        public void OnItemSelected(Issue issue)
        {
            this._parent.ShowIssueDetail(issue, this._boardProject);
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", this._boardProject.Name);
        }

        public ObservableCollection<Issue> IssueList
        {
            get { return this._issueList; }
            set { this._issueList = value; }
        }

        public ObservableCollection<Sprint> SprintList
        {
            get { return this._sprintList; }
            set { this._sprintList = value; }
        }

        public Sprint SelectedSprint
        {
            get { return this._selectedSprint; }
            set {
                this._selectedSprint = value;

                if (this._selectedSprint.Id.Equals("0"))
                {
                    this.GetIssuesAsync();
                }
                else
                {
                    this.GetIssuesBySprintAsync();
                }
                OnPropertyChanged("SelectedSprint");
            }
        }
    }
}
