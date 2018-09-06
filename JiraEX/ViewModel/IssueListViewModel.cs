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
    public class IssueListViewModel : ViewModelBase
    {
        private IIssueService _issueService;
        private ISprintService _sprintService;

        private JiraToolWindowNavigatorViewModel _parent;

        private BoardProject _boardProject;

        private ObservableCollection<Issue> _issueList;
        private ObservableCollection<Sprint> _sprintList;

        public DelegateCommand IssueSelectedCommand { get; private set; }

        public IssueListViewModel(JiraToolWindowNavigatorViewModel parent, BoardProject boardProject)
        {
            this._issueService = new IssueService();
            this._sprintService = new SprintService();

            this._parent = parent;

            this._boardProject = boardProject;

            this.IssueList = new ObservableCollection<Issue>();
            this.SprintList = new ObservableCollection<Sprint>();

            this.IssueSelectedCommand = new DelegateCommand(OnItemSelected);
            OleMenuCommandService service = JiraPackage.Mcs;

            GetIssuesAsync();
            GetSprintsAsync();

            this.IssueList.CollectionChanged += this.OnCollectionChanged;
            this.SprintList.CollectionChanged += this.OnCollectionChanged;
        }

        private async void GetIssuesAsync()
        {
            System.Threading.Tasks.Task<IssueList> issueTask = this._issueService.GetAllIssuesOfBoardAsync(this._boardProject.Id);

            var issueList = await issueTask as IssueList;

            foreach (Issue i in issueList.Issues)
            {
                this.IssueList.Add(i);
            }
        }

        private async void GetSprintsAsync()
        {
            System.Threading.Tasks.Task<SprintList> sprintTask = this._sprintService.GetAllSprintsOfBoardtAsync(this._boardProject.Id);

            var sprintList = await sprintTask as SprintList;

            foreach (Sprint s in sprintList.Values)
            {
                this.SprintList.Add(s);
            }
        }

        private void OnItemSelected(object sender)
        {

        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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

    }
}
