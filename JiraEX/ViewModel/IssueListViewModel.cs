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

        private IJiraToolWindowNavigatorViewModel _parent;

        private Project _project;

        private ObservableCollection<Issue> _issueList;

        private string _subtitle;
        private bool _canCreateIssue = false;

        public DelegateCommand CreateIssueCommand { get; private set; }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService)
        {
            this._issueService = issueService;

            this._parent = parent;

            this.IssueList = new ObservableCollection<Issue>();

            this.CreateIssueCommand = new DelegateCommand(RedirectCreateIssue);

            OleMenuCommandService service = JiraPackage.Mcs;
        }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, Project project) : this(parent, issueService)
        {
            this._project = project;
            this._subtitle = this._project.Name;
            this.CanCreateIssue = true;

            GetIssuesAsync();

            this.IssueList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }


        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, string filter) : this(parent, issueService)
        {
            GetIssuesAsync(filter);
            this._subtitle = filter;

            this.IssueList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }

        private void RedirectCreateIssue(object sender)
        {
            this._parent.CreateIssue(this._project);
        }

        private async void GetIssuesAsync()
        {
            Task<IssueList> issueTask = this._issueService.GetAllIssuesOfProjectAsync(this._project.Key);

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

        public void OnItemSelected(Issue issue)
        {
            this._parent.ShowIssueDetail(issue, this._project);
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

        public bool CanCreateIssue
        {
            get { return this._canCreateIssue; }
            set
            {
                this._canCreateIssue = value;
                OnPropertyChanged("CanCreateIssue");
            }
        }
    }
}
