using AtlassianConnector.Model.Exceptions;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class ProjectListViewModel : ViewModelBase, ITitleable, IReinitializable
    {
        private IProjectService _projectService;
        private IBoardService _boardService;

        private IJiraToolWindowNavigatorViewModel _parent;

        private bool _noProjects = false;

        private ObservableCollection<Project> _projectList;
        private ProjectCreatableList _projectCreatableList;

        public DelegateCommand ProjectSelectedCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ProjectListViewModel(IJiraToolWindowNavigatorViewModel parent, IProjectService projectService, IBoardService boardService)
        {
            this._projectService = projectService;
            this._boardService = boardService;

            this._parent = parent;
            this._parent.SetRefreshCommand(RefreshProjects);

            this.ProjectList = new ObservableCollection<Project>();

            this.ProjectSelectedCommand = new DelegateCommand(OnItemSelected);
            OleMenuCommandService service = JiraPackage.Mcs;

            GetProjectsAsync();

            this.ProjectList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }

        private void RefreshProjects(object sender, EventArgs e)
        {
            GetProjectsAsync();
        }

        private async void GetProjectsAsync()
        {
            this._parent.StartLoading();

            this.ProjectList.Clear();

            Task<ProjectList> projectTask = this._projectService.GetAllProjectsAsync();

            try
            {
                var projectList = await projectTask as ProjectList;

                this._projectCreatableList = await this._projectService.GetAllProjectsCreatableIssueTypesAsync();

                if (projectList.Count > 0)
                {
                    foreach (Project p in projectList)
                    {

                        //Fetching Creatable IssueTypes for each project
                        foreach (ProjectCreatable pc in this._projectCreatableList.Projects)
                        {
                            if (p.Id.Equals(pc.Id))
                            {
                                p.CreatableIssueTypesList = pc.Issuetypes;
                            }
                        }
                        //end fetching

                        this.ProjectList.Add(p);
                    }
                }
                else
                {
                    this.NoProjects = true;
                }

                this._parent.StopLoading();

                HideErrorMessages(this._parent);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }
        }

        public void OnItemSelected(object sender)
        {
            Project project = sender as Project;

            this._parent.ShowIssuesOfProject(project);
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Projects");
        }

        public void Reinitialize()
        {
            this._parent.SetRefreshCommand(RefreshProjects);

            GetProjectsAsync();
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

        public bool NoProjects
        {
            get { return this._noProjects; }
            set
            {
                this._noProjects = value;
                OnPropertyChanged("NoProjects");
            }
        }
    }
}
