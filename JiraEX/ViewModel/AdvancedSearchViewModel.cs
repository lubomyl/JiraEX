using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
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

        private Sprint _selectedSprint;
        private User _selectedAssignee;
        private Priority _selectedPriority;
        private Status _selectedStatus;
        private Project _selectedProject;
        private string _searchText;

        private ObservableCollection<Sprint> _sprintList;
        private ObservableCollection<User> _assigneeList;
        private ObservableCollection<Priority> _priorityList;
        private ObservableCollection<Status> _statusList;
        private ObservableCollection<Project> _projectList;

        public AdvancedSearchViewModel(JiraToolWindowNavigatorViewModel parent)
        {
            this._parent = parent;
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

        public Priority SelectedPriority
        {
            get { return this._selectedPriority; }
            set
            {
                this._selectedPriority = value;
                OnPropertyChanged("SelectedPriority");
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
    }
}
