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
    public class ProjectListViewModel : ViewModelBase
    {
        private IProjectService _projectService;

        private JiraToolWindowNavigatorViewModel _parent;

        private ObservableCollection<Project> _projectList;

        public DelegateCommand ProjectSelectedCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ProjectListViewModel(JiraToolWindowNavigatorViewModel parent)
        {
            this._projectService = new ProjectService();

            this._parent = parent;
            this.ProjectList = new ObservableCollection<Project>();

            this.ProjectSelectedCommand = new DelegateCommand(OnItemSelected);
            OleMenuCommandService service = JiraPackage.Mcs;

            GetProjectsAsync();

            this.ProjectList.CollectionChanged += this.OnCollectionChanged;
        }

        private async void GetProjectsAsync()
        {
            System.Threading.Tasks.Task<ProjectList> projectTask = this._projectService.GetAllProjectsAsync();

            var projectList = await projectTask as ProjectList;

            foreach (Project p in projectList.Results)
            {
                this.ProjectList.Add(p);
            }
        }

        private void OnItemSelected(object sender)
        {

        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public ObservableCollection<Project> ProjectList
        {
            get { return this._projectList; }
            set { this._projectList = value; }
        }
    }
}
