using ConfluenceEX.Command;
using JiraEX.Helper;
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
        private IProjectService _projectService;
        private ISprintService _sprintService;
        private IBoardService _boardService;

        private string _searchText;

        private bool _isAssignedToMe;
        private bool _isUnassigned;

        private bool _isEditingProjects;
        private bool _isEditingSprints;
        private bool _isEditingPriorities;
        private bool _isEditingStatuses;

        private ObservableCollection<Sprint> _sprintList;
        private ObservableCollection<Priority> _priorityList;
        private ObservableCollection<Status> _statusList;
        private ObservableCollection<Project> _projectList;
        private List<BoardProject> _boardList;

        private ObservableCollection<Issue> _issueList;

        public DelegateCommand CheckedPriorityCommand { get; set; }
        public DelegateCommand CheckedStatusCommand { get; set; }
        public DelegateCommand CheckedProjectCommand { get; set; }
        public DelegateCommand CheckedSprintCommand { get; set; }

        public DelegateCommand EditProjectsCommand { get; set; }
        public DelegateCommand CancelEditProjectsCommand { get; set; }
        public DelegateCommand EditSprintsCommand { get; set; }
        public DelegateCommand CancelEditSprintsCommand { get; set; }
        public DelegateCommand EditPrioritiesCommand { get; set; }
        public DelegateCommand CancelEditPrioritiesCommand { get; set; }
        public DelegateCommand EditStatusesCommand { get; set; }
        public DelegateCommand CancelEditStatusesCommand { get; set; }

        Task<IssueList> issueTask;

        public AdvancedSearchViewModel(IJiraToolWindowNavigatorViewModel parent, IPriorityService priorityService, 
            IIssueService issueService, IProjectService projectService, ISprintService sprintService, IBoardService boardService)
        {
            this._parent = parent;

            this._priorityService = priorityService;
            this._issueService = issueService;
            this._projectService = projectService;
            this._sprintService = sprintService;
            this._boardService = boardService;

            this.PriorityList = new ObservableCollection<Priority>();
            this.StatusList = new ObservableCollection<Status>();
            this.ProjectList = new ObservableCollection<Project>();
            this.IssueList = new ObservableCollection<Issue>();
            this._boardList = new List<BoardProject>();
            this.SprintList = new ObservableCollection<Sprint>();

            this.CheckedPriorityCommand = new DelegateCommand(CheckedPriority);
            this.CheckedStatusCommand = new DelegateCommand(CheckedStatus);
            this.CheckedProjectCommand = new DelegateCommand(CheckedProject);
            this.CheckedSprintCommand = new DelegateCommand(CheckedSprint);

            this.EditProjectsCommand = new DelegateCommand(EditProjects);
            this.CancelEditProjectsCommand = new DelegateCommand(CancelEditProjects);

            this.EditSprintsCommand = new DelegateCommand(EditSprints);
            this.CancelEditSprintsCommand = new DelegateCommand(CancelEditSprints);

            this.EditPrioritiesCommand = new DelegateCommand(EditPriorities);
            this.CancelEditPrioritiesCommand = new DelegateCommand(CancelEditPriorities);

            this.EditStatusesCommand = new DelegateCommand(EditStatuses);
            this.CancelEditStatusesCommand = new DelegateCommand(CancelEditStatuses);

            GetIssuesAsync();
            GetPrioritiesAsync();
            GetStatusesAsync();
            GetProjectsAsync();
            GetBoardsAsync();
        }

        private async void GetIssuesAsync()
        {
            string jql = JqlBuilder.Build(this.SprintList.ToArray(), this.IsAssignedToMe, this.IsUnassigned, 
                this.PriorityList.ToArray(), this.StatusList.ToArray(), this.ProjectList.ToArray(), this.SearchText);

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

        private async void GetStatusesAsync()
        {
            Task<ProjectList> projectTask = this._projectService.GetAllProjectsAsync();

            var projectList = await projectTask as ProjectList;

            this.StatusList.Clear();

            foreach(Project p in projectList)
            {
                Task<StatusList> statusTask = this._projectService.GetAllStatusesByProjectKeyAsync(p.Key);

                var statusList = await statusTask as StatusList;

                foreach(Status s in statusList[0].Statuses)
                {
                    if (this.StatusList.Count > 0)
                    {
                        Status contains = this.StatusList.FirstOrDefault(status => status.Name.Equals(s.Name));

                        if (contains == null)
                        {
                            this.StatusList.Add(s);
                        }
                    } else
                    {
                        this.StatusList.Add(s);
                    }
                }
            }
        }

        private async void GetProjectsAsync()
        {
            Task<ProjectList> projectTask = this._projectService.GetAllProjectsAsync();

            var projectList = await projectTask as ProjectList;

            this.ProjectList.Clear();

            foreach (Project p in projectList)
            {
                this.ProjectList.Add(p);
            }
        }

        private async void GetBoardsAsync()
        {
            Task<BoardProjectList> boardsTask = this._boardService.GetAllBoardsAsync();
            var boardsList = await boardsTask as BoardProjectList;

            this._boardList.Clear();

            foreach (BoardProject b in boardsList.Values)
            {
                this._boardList.Add(b);
            }

            GetSprintsAsync();
        }

        private async void GetSprintsAsync()
        {
            this.SprintList.Clear();

            foreach (BoardProject bp in this._boardList)
            {
                Task<SprintList> sprintsTask = this._boardService.GetAllSprintsByBoardIdAsync(bp.Id);
                var sprintsList = await sprintsTask as SprintList;

                if (sprintsList != null)
                {
                    foreach (Sprint s in sprintsList.Values)
                    {
                        if (!this.SprintList.Any(sprint => sprint.Id == s.Id))
                        {
                            this.SprintList.Add(s);
                        }
                    }
                }
            }
        }

        private void CheckedPriority(object sender)
        {
            Priority priority = sender as Priority;

            OnPropertyChanged("SelectedPriorities");

            GetIssuesAsync();
        }

        private void CheckedStatus(object sender)
        {
            Status status = sender as Status;

            OnPropertyChanged("SelectedStatuses");

            GetIssuesAsync();
        }

        private void CheckedProject(object sender)
        {
            Project project = sender as Project;

            OnPropertyChanged("SelectedProjects");

            GetIssuesAsync();
        }

        private void CheckedSprint(object sender)
        {
            Sprint sprint = sender as Sprint;

            OnPropertyChanged("SelectedSprints");

            GetIssuesAsync();
        }

        public void OnItemSelected(Issue issue)
        {
            this._parent.ShowIssueDetail(issue, null);
        }

        private void EditProjects(object sender)
        {
            this.IsEditingProjects = true;
        }

        private void CancelEditProjects(object sender)
        {
            this.IsEditingProjects = false;
        }

        private void EditSprints(object sender)
        {
            this.IsEditingSprints = true;
        }

        private void CancelEditSprints(object sender)
        {
            this.IsEditingSprints = false;
        }

        private void EditPriorities(object sender)
        {
            this.IsEditingPriorities = true;
        }

        private void CancelEditPriorities(object sender)
        {
            this.IsEditingPriorities = false;
        }

        private void EditStatuses(object sender)
        {
            this.IsEditingStatuses = true;
        }

        private void CancelEditStatuses(object sender)
        {
            this.IsEditingStatuses = false;
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Advanced search");
        }

        public bool IsAssignedToMe
        {
            get { return this._isAssignedToMe; }
            set
            {
                this._isAssignedToMe = value;
                OnPropertyChanged("IsAssignedToMe");
                GetIssuesAsync();
            }
        }

        public bool IsUnassigned
        {
            get { return this._isUnassigned; }
            set
            {
                this._isUnassigned = value;
                OnPropertyChanged("IsUnassigned");
                GetIssuesAsync();
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

        public bool IsEditingProjects
        {
            get { return this._isEditingProjects; }
            set
            {
                this._isEditingProjects = value;
                OnPropertyChanged("IsEditingProjects");
            }
        }

        public bool IsEditingSprints
        {
            get { return this._isEditingSprints; }
            set
            {
                this._isEditingSprints= value;
                OnPropertyChanged("IsEditingSprints");
            }
        }

        public bool IsEditingPriorities
        {
            get { return this._isEditingPriorities; }
            set
            {
                this._isEditingPriorities = value;
                OnPropertyChanged("IsEditingPriorities");
            }
        }

        public bool IsEditingStatuses
        {
            get { return this._isEditingStatuses; }
            set
            {
                this._isEditingStatuses = value;
                OnPropertyChanged("IsEditingStatuses");
            }
        }

        public string SelectedProjects
        {
            get
            {
                string ret = "";

                foreach (Project p in this.ProjectList)
                {
                    if (p.CheckedStatus)
                    {
                        if (!ret.Equals(""))
                        {
                            ret += ", " + p.Name;
                        }
                        else
                        {
                            ret += p.Name;
                        }
                    }
                }

                if (ret.Equals(""))
                {
                    ret = "None";
                }

                return ret;
            }
        }

        public string SelectedSprints
        {
            get
            {
                string ret = "";

                foreach (Sprint s in this.SprintList)
                {
                    if (s.CheckedStatus)
                    {
                        if (!ret.Equals(""))
                        {
                            ret += ", " + s.Name;
                        }
                        else
                        {
                            ret += s.Name;
                        }
                    }
                }

                if (ret.Equals(""))
                {
                    ret = "None";
                }

                return ret;
            }
        }

        public string SelectedPriorities
        {
            get
            {
                string ret = "";

                foreach (Priority p in this.PriorityList)
                {
                    if (p.CheckedStatus)
                    {
                        if (!ret.Equals(""))
                        {
                            ret += ", " + p.Name;
                        }
                        else
                        {
                            ret += p.Name;
                        }
                    }
                }

                if (ret.Equals(""))
                {
                    ret = "None";
                }

                return ret;
            }
        }

        public string SelectedStatuses
        {
            get
            {
                string ret = "";

                foreach (Status s in this.StatusList)
                {
                    if (s.CheckedStatus)
                    {
                        if (!ret.Equals(""))
                        {
                            ret += ", " + s.Name;
                        }
                        else
                        {
                            ret += s.Name;
                        }
                    }
                }

                if (ret.Equals(""))
                {
                    ret = "None";
                }

                return ret;
            }
        }
    }
}
