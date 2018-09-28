using ConfluenceEX.Command;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class AdvancedSearchViewModel : ViewModelBase, ITitleable
    {

        private IJiraToolWindowNavigatorViewModel _parent;

        private IPriorityService _priorityService;
        private IIssueService _issueService;

        private Sprint _selectedSprint;
        private User _selectedAssignee;
        private Status _selectedStatus;
        private Project _selectedProject;
        private string _searchText;

        private ObservableCollection<Sprint> _sprintList;
        private ObservableCollection<User> _assigneeList;
        private ObservableCollection<Priority> _priorityList;
        private ObservableCollection<Status> _statusList;
        private ObservableCollection<Project> _projectList;

        private ObservableCollection<Issue> _issueList;

        public DelegateCommand CheckedPriorityCommand { get; set; }

        Task<IssueList> issueTask;

        public AdvancedSearchViewModel(IJiraToolWindowNavigatorViewModel parent, IPriorityService priorityService, IIssueService issueService)
        {
            this._parent = parent;

            this._priorityService = priorityService;
            this._issueService = issueService;

            this.PriorityList = new ObservableCollection<Priority>();
            this.IssueList = new ObservableCollection<Issue>();


            this.CheckedPriorityCommand = new DelegateCommand(CheckedPriority);

            GetIssuesAsync();
            GetPrioritiesAsync();
        }

        private async void GetIssuesAsync()
        {
            string jql = "";
            string priorityJql = "";

            foreach (Priority p in PriorityList)
            {
                if (p.CheckedStatus)
                {
                    if (priorityJql.Equals(""))
                    {
                        priorityJql += "\"" + p.Name + "\"";
                    } else
                    {
                        priorityJql += "," + "\"" + p.Name + "\"";
                    }
                }
            }

            if (!priorityJql.Equals(""))
            {
                jql += $"priority in ({priorityJql})";
            }

            if (this.SearchText != null)
            {
                if (!jql.Equals(""))
                {
                    jql += $" AND text~\"{this.SearchText}\"";
                }
                else
                {
                    jql += $"text~\"{this.SearchText}\"";
                }
            }

            this.issueTask = this._issueService.GetAllIssuesByJqlAsync(jql);

            var issueList = await issueTask as IssueList;

            this.IssueList.Clear();

            foreach (Issue i in issueList.Issues)
            {
                this.IssueList.Add(i);
            }
        }

        private async void GetPrioritiesAsync()
        {
            Task<PriorityList> priorityTask = this._priorityService.GetAllPrioritiesAsync();

            var priorityList = await priorityTask as PriorityList;

            this.PriorityList.Clear();

            foreach (Priority p in priorityList)
            {
                this.PriorityList.Add(p);
            }
        }

        private void CheckedPriority(object sender)
        {
            Priority priority = sender as Priority;

            GetIssuesAsync();
        }

        public void OnItemSelected(Issue issue)
        {
            this._parent.ShowIssueDetail(issue, null);
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Advanced search");
        }

        public Sprint SelectedSprint
        {
            get { return this._selectedSprint; }
            set
            {
                this._selectedSprint = value;
                OnPropertyChanged("SelectedSprint");
            }
        }

        public User SelectedAssignee
        {
            get { return this._selectedAssignee; }
            set
            {
                this._selectedAssignee = value;
                OnPropertyChanged("SelectedAssignee");
            }
        }

        public Status SelectedStatus
        {
            get { return this._selectedStatus; }
            set
            {
                this._selectedStatus = value;
                OnPropertyChanged("SelectedStatus");
            }
        }

        public Project SelectedProject
        {
            get { return this._selectedProject; }
            set
            {
                this._selectedProject = value;
                OnPropertyChanged("SelectedProject");
            }
        }

        public string SearchText
        {
            get { return this._searchText; }
            set
            {
                this._searchText = value;
                OnPropertyChanged("SearchText");
                GetIssuesAsync();
            }
        }

        public ObservableCollection<Sprint> SprintList
        {
            get { return this._sprintList; }
            set
            {
                this._sprintList = value;
                OnPropertyChanged("SprintList");
            }
        }

        public ObservableCollection<User> AssigneeList
        {
            get { return this._assigneeList; }
            set
            {
                this._assigneeList = value;
                OnPropertyChanged("AssigneeList");
            }
        }

        public ObservableCollection<Priority> PriorityList
        {
            get { return this._priorityList; }
            set
            {
                this._priorityList = value;
                OnPropertyChanged("PriorityList");
            }
        }

        public ObservableCollection<Status> StatusList
        {
            get { return this._statusList; }
            set
            {
                this._statusList = value;
                OnPropertyChanged("StatusList");
            }
        }

        public ObservableCollection<Project> ProjectList
        {
            get { return this._projectList; }
            set
            {
                this._projectList = value;
                OnPropertyChanged("ProjectList");
            }
        }

        public ObservableCollection<Issue> IssueList
        {
            get { return this._issueList; }
            set
            {
                this._issueList = value;
                OnPropertyChanged("IssueList");
            }
        }
    }
}
