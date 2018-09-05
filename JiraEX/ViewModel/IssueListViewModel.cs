﻿using ConfluenceEX.Command;
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

        private JiraToolWindowNavigatorViewModel _parent;

        private Project _project;

        private ObservableCollection<Issue> _issueList;

        public DelegateCommand IssueSelectedCommand { get; private set; }

        public IssueListViewModel(JiraToolWindowNavigatorViewModel parent, Project project)
        {
            this._issueService = new IssueService();

            this._parent = parent;

            this._project = project;

            this.IssueList = new ObservableCollection<Issue>();

            this.IssueSelectedCommand = new DelegateCommand(OnItemSelected);
            OleMenuCommandService service = JiraPackage.Mcs;

            GetIssuesAsync();

            this.IssueList.CollectionChanged += this.OnCollectionChanged;
        }

        private async void GetIssuesAsync()
        {
            System.Threading.Tasks.Task<IssueList> issueTask = this._issueService.GetAllIssuesOfProjectAsync(this._project.Key);

            var issueList = await issueTask as IssueList;

            foreach (Issue i in issueList.Issues)
            {
                this.IssueList.Add(i);
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

    }
}
