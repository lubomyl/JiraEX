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

        private string _subtitle;
        private bool _canCreateIssue = false;
        private bool _canFilterBySprint = false;

        public DelegateCommand CreateIssueCommand { get; private set; }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, ISprintService sprintService)
        {
            this._issueService = issueService;
            this._sprintService = sprintService;

            this._parent = parent;

            this.IssueList = new ObservableCollection<Issue>();

            this.CreateIssueCommand = new DelegateCommand(RedirectCreateIssue);

            OleMenuCommandService service = JiraPackage.Mcs;
        }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, ISprintService sprintService, BoardProject boardProject) : this(parent, issueService, sprintService)
        {
            this._boardProject = boardProject;
            this._subtitle = this._boardProject.Name;
            this.CanCreateIssue = true;

            this.SprintList = new ObservableCollection<Sprint>();

            this._defaultSprintSelected = new Sprint("0", "All sprints");

            GetIssuesAsync();
            GetSprintsAsync();

            this.IssueList.CollectionChanged += this.OnCollectionChanged;
            this.SprintList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }


        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, ISprintService sprintService, string filter) : this(parent, issueService, sprintService)
        {
            GetIssuesAsync(filter);
            this._subtitle = filter;

            this.IssueList.CollectionChanged += this.OnCollectionChanged;

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

        private async void GetIssuesAsync(string filter)
        {
            Task<IssueList> issueTask = this._issueService.GetAllIssuesByJqlAsync(filter);

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
            try
            {
                //TODO catch exception about not being able to support sprints
                Task<SprintList> sprintTask = this._sprintService.GetAllSprintsOfBoardtAsync(this._boardProject.Id);

                var sprintList = sprintTask.Result as SprintList;

                this.SprintList.Add(this._defaultSprintSelected);
                this.SelectedSprint = this.SprintList[0];

                foreach (Sprint s in sprintList.Values)
                {
                    this.SprintList.Add(s);
                }

                if(this.SprintList.Count > 0)
                {
                    this.CanFilterBySprint = true;
                }
            }
            catch (Exception ex)
            {

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
            this._parent.SetPanelTitles("JiraEX", this._subtitle);
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

        public bool CanCreateIssue
        {
            get { return this._canCreateIssue; }
            set
            {
                this._canCreateIssue = value;
                OnPropertyChanged("CanCreateIssue");
            }
        }

        public bool CanFilterBySprint
        {
            get { return this._canFilterBySprint; }
            set
            {
                this._canFilterBySprint = value;
                OnPropertyChanged("CanFilterBySprint");
            }
        }
    }
}
