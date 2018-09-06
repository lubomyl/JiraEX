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
        private IBoardService _boardService;

        private JiraToolWindowNavigatorViewModel _parent;

        private ObservableCollection<BoardProject> _boardProjectList;

        public DelegateCommand ProjectSelectedCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ProjectListViewModel(JiraToolWindowNavigatorViewModel parent)
        {
            this._projectService = new ProjectService();
            this._boardService = new BoardService();

            this._parent = parent;
            this.BoardProjectList = new ObservableCollection<BoardProject>();

            this.ProjectSelectedCommand = new DelegateCommand(OnItemSelected);
            OleMenuCommandService service = JiraPackage.Mcs;

            //GetProjectsAsync();
            GetBoardsAsync();

            this.BoardProjectList.CollectionChanged += this.OnCollectionChanged;
        }

        /*private async void GetProjectsAsync()
        {
            System.Threading.Tasks.Task<ProjectList> projectTask = this._projectService.GetAllProjectsAsync();

            var projectList = await projectTask as ProjectList;

            foreach (Project p in projectList)
            {
                this.BoardList.Add(p);
            }
        }*/

        private async void GetBoardsAsync()
        {
            System.Threading.Tasks.Task<BoardList> boardTask = this._boardService.GetAllBoards();

            var boardList = await boardTask as BoardList;

            foreach (BoardProject b in boardList.Values)
            {
                this.BoardProjectList.Add(b);
            }
        }

        private void OnItemSelected(object sender)
        {
            BoardProject boardProject = sender as BoardProject;

            this._parent.ShowIssuesOfProject(boardProject);
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public ObservableCollection<BoardProject> BoardProjectList
        {
            get { return this._boardProjectList; }
            set { this._boardProjectList = value; }
        }
    }
}
