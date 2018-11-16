using AtlassianConnector.Model.Exceptions;
using ConfluenceEX.Command;
using ConfluenceEX.Helper;
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
    public class IssueListViewModel : ViewModelBase, ITitleable, IReinitializable
    {
        private IIssueService _issueService;

        private IJiraToolWindowNavigatorViewModel _parent;

        private bool _noIssues = false;
        private bool _noIssuesSearch = false;

        private Project _project;
        private string _filter;
        private string _searchString;

        private int _startAt = 0;
        private int _maxResults = 0;

        private bool _hasNext = false;
        private bool _hasPrevious = false;
        private bool _isEditingAttributes;

        private enum IssueListType
        {
            normal,
            filter,
            quickSearch
        }

        private IssueListType _type;

        private ObservableCollection<Issue> _issueList;
        private ObservableCollection<MyAttribute> _attributesList;

        private string _subtitle;
        private bool _canCreateIssue = false;

        public DelegateCommand CreateIssueCommand { get; private set; }

        public DelegateCommand CheckedAttributeCommand { get; set; }
        public DelegateCommand EditAttributesCommand { get; set; }
        public DelegateCommand CancelEditAttributesCommand { get; set; }

        public DelegateCommand GetNextIssuesCommand { get; set; }
        public DelegateCommand GetPreviousIssuesCommand { get; set; }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService)
        {
            this._issueService = issueService;

            this._parent = parent;

            this.IssueList = new ObservableCollection<Issue>();
            AttributesList = new ObservableCollection<MyAttribute>();

            this.CreateIssueCommand = new DelegateCommand(RedirectCreateIssue);

            this.CheckedAttributeCommand = new DelegateCommand(CheckedAttribute);
            this.EditAttributesCommand = new DelegateCommand(EditAttributes);
            this.CancelEditAttributesCommand = new DelegateCommand(CancelEditAttributes);

            this.GetNextIssuesCommand = new DelegateCommand(GetNextIssues);
            this.GetPreviousIssuesCommand = new DelegateCommand(GetPreviousIssues);

            InitializeAttributes();

            OleMenuCommandService service = JiraPackage.Mcs;
        }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, Project project) : this(parent, issueService)
        {
            this._type = IssueListType.normal;

            this._project = project;
            this._subtitle = this._project.Name;
            this.CanCreateIssue = true;

            this._parent.SetRefreshCommand(RefreshIssues);

            GetIssuesAsync(this._startAt);

            this.IssueList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }


        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, string filter) : this(parent, issueService)
        {
            this._type = IssueListType.filter;
            this._filter = filter;

            this._subtitle = filter;

            this._parent.SetRefreshCommand(RefreshFilteredIssues);

            GetIssuesAsync(this._startAt, filter);

            this.IssueList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }

        public IssueListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService, bool quickSearch, string searchString) : this(parent, issueService)
        {
            this._type = IssueListType.quickSearch;

            this._searchString = searchString;

            this._subtitle = "Search string \"" + searchString + "\""; ;

            this._parent.SetRefreshCommand(RefreshQuickSearchIssues);

            GetIssuesQuickSearchAsync(this._startAt, searchString);

            this.IssueList.CollectionChanged += this.OnCollectionChanged;

            SetPanelTitles();
        }

        private void RefreshIssues(object sender, EventArgs e)
        {
            GetIssuesAsync(this._startAt);
        }

        private void RefreshFilteredIssues(object sender, EventArgs e)
        {
            GetIssuesAsync(this._startAt, this._filter);
        }

        private void RefreshQuickSearchIssues(object sender, EventArgs e)
        {
            GetIssuesQuickSearchAsync(this._startAt, this._searchString);
        }

        private void RedirectCreateIssue(object sender)
        {
            this._parent.ShowCreateIssue(this._project);
        }

        private void GetNextIssues(object sender) {
            this._startAt += this._maxResults;

            switch (this._type)
            {
                case IssueListType.normal:
                    this.GetIssuesAsync(this._startAt);
                    break;
                case IssueListType.filter:
                    this.GetIssuesAsync(this._startAt, this._filter);
                    break;
                case IssueListType.quickSearch:
                    this.GetIssuesQuickSearchAsync(this._startAt, this._searchString);
                    break;
            }
        }

        private void GetPreviousIssues(object sender)
        {
            this._startAt -= this._maxResults;

            switch (this._type)
            {
                case IssueListType.normal:
                    this.GetIssuesAsync(this._startAt);
                    break;
                case IssueListType.filter:
                    this.GetIssuesAsync(this._startAt, this._filter);
                    break;
                case IssueListType.quickSearch:
                    this.GetIssuesQuickSearchAsync(this._startAt, this._searchString);
                    break;
            }
        }

        private async void GetIssuesAsync(int startAt)
        {
            this._parent.StartLoading();

            Task<IssueListPaged> issueTask = this._issueService.GetAllIssuesOfProjectAsync(startAt, this._project.Key);

            try { 
                var issueList = await issueTask as IssueListPaged;

                if (issueList.Issues.Count != 0)
                {
                    this.NoIssues = false;

                    this.IssueList.Clear();

                    foreach (Issue i in issueList.Issues)
                    {
                        this.IssueList.Add(i);
                    }  
                } else
                {
                    this.NoIssues = true;
                }

                ProcessBoundaries(issueList);

                this._parent.StopLoading();

                HideErrorMessages(this._parent);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);

                this._parent.StopLoading();
            }
        }

        private async void GetIssuesAsync(int startAt, string filter)
        {
            this._parent.StartLoading();

            Task<IssueListPaged> issueTask = this._issueService.GetAllIssuesByJqlAsync(this._startAt, filter);

            try { 
                var issueList = await issueTask as IssueListPaged;

                if (issueList.Issues.Count != 0)
                {
                    this.NoIssues = false;

                    this.IssueList.Clear();

                    foreach (Issue i in issueList.Issues)
                    {
                        this.IssueList.Add(i);
                    }
                } else
                {
                    this.NoIssues = true;
                }

                ProcessBoundaries(issueList);

                this._parent.StopLoading();

                HideErrorMessages(this._parent);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);

                this._parent.StopLoading();
            }
        }

        private async void GetIssuesQuickSearchAsync(int startAt, string searchString)
        {
            this._parent.StartLoading();

            Task<IssueListPaged> issueTask = this._issueService.GetAllIssuesByTextContainingAsync(this._startAt, searchString);

            try
            {
                var issueList = await issueTask as IssueListPaged;

                if (issueList.Issues.Count != 0)
                {
                    this.NoIssuesSearch = false;

                    this.IssueList.Clear();

                    foreach (Issue i in issueList.Issues)
                    {
                        this.IssueList.Add(i);
                    }
                } else
                {
                    this.NoIssuesSearch = true;
                }

                ProcessBoundaries(issueList);

                this._parent.StopLoading();

                HideErrorMessages(this._parent);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);

                this._parent.StopLoading();
            }
        }

        private void ProcessBoundaries(IssueListPaged issueList)
        {
            int startAt = issueList.StartAt;
            int maxResults = issueList.MaxResults;

            if (maxResults == issueList.Issues.Count)
            {
                this.HasNext = true;
            }
            else
            {
                this.HasNext = false;
            }

            if (startAt != 0)
            {
                this.HasPrevious = true;
            }
            else
            {
                this.HasPrevious = false;
            }

            this._startAt = startAt;
            this._maxResults = maxResults;
        }

        public void OnItemSelected(Issue issue)
        {
            this._parent.ShowIssueDetail(issue, this._project);
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private void CheckedAttribute(object sender)
        {
            OnPropertyChanged("SelectedAttributes");

            RefreshShowedAttributes(sender);
        }

        private void RefreshShowedAttributes(object sender)
        {
            MyAttribute attribute = sender as MyAttribute;

            switch (attribute.Name)
            {
                case "Type":
                    OnPropertyChanged("TypeAttribute");
                    UserSettingsHelper.WriteToUserSettings("TypeAttribute", this.AttributesList[0].CheckedStatus);
                    break;
                case "Status":
                    OnPropertyChanged("StatusAttribute");
                    UserSettingsHelper.WriteToUserSettings("StatusAttribute", this.AttributesList[1].CheckedStatus);
                    break;
                case "Created":
                    OnPropertyChanged("CreatedAttribute");
                    UserSettingsHelper.WriteToUserSettings("CreatedAttribute", this.AttributesList[2].CheckedStatus);
                    break;
                case "Updated":
                    OnPropertyChanged("UpdatedAttribute");
                    UserSettingsHelper.WriteToUserSettings("UpdatedAttribute", this.AttributesList[3].CheckedStatus);
                    break;
                case "Assignee":
                    OnPropertyChanged("AssigneeAttribute");
                    UserSettingsHelper.WriteToUserSettings("AssigneeAttribute", this.AttributesList[4].CheckedStatus);
                    break;
                case "Summary":
                    OnPropertyChanged("SummaryAttribute");
                    UserSettingsHelper.WriteToUserSettings("SummaryAttribute", this.AttributesList[5].CheckedStatus);
                    break;
                case "Priority":
                    OnPropertyChanged("PriorityAttribute");
                    UserSettingsHelper.WriteToUserSettings("PriorityAttribute", this.AttributesList[6].CheckedStatus);
                    break;
            }
        }

        private void EditAttributes(object sender)
        {
            this.IsEditingAttributes = true;
        }

        private void CancelEditAttributes(object sernder)
        {
            this.IsEditingAttributes = false;
        }

        private void InitializeAttributes()
        {   
            this.AttributesList.Add(new MyAttribute("Type", UserSettingsHelper.ReadBoolFromUserSettings("TypeAttribute")));
            this.AttributesList.Add(new MyAttribute("Status", UserSettingsHelper.ReadBoolFromUserSettings("StatusAttribute")));
            this.AttributesList.Add(new MyAttribute("Created", UserSettingsHelper.ReadBoolFromUserSettings("CreatedAttribute")));
            this.AttributesList.Add(new MyAttribute("Updated", UserSettingsHelper.ReadBoolFromUserSettings("UpdatedAttribute")));
            this.AttributesList.Add(new MyAttribute("Assignee", UserSettingsHelper.ReadBoolFromUserSettings("AssigneeAttribute")));
            this.AttributesList.Add(new MyAttribute("Summary", UserSettingsHelper.ReadBoolFromUserSettings("SummaryAttribute")));
            this.AttributesList.Add(new MyAttribute("Priority", UserSettingsHelper.ReadBoolFromUserSettings("PriorityAttribute")));
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", this._subtitle);
        }

        public void Reinitialize()
        {
            if (this._filter != null)
            {
                this._parent.SetRefreshCommand(RefreshFilteredIssues);
                GetIssuesAsync(this._startAt, this._filter);
            }
            else if(this._searchString != null)
            {
                this._parent.SetRefreshCommand(RefreshQuickSearchIssues);
                GetIssuesQuickSearchAsync(this._startAt, this._searchString);
            }
            else
            {
                this._parent.SetRefreshCommand(RefreshIssues);
                GetIssuesAsync(this._startAt);
            }
        }

        public ObservableCollection<Issue> IssueList
        {
            get { return this._issueList; }
            set { this._issueList = value; }
        }

        public ObservableCollection<MyAttribute> AttributesList
        {
            get { return this._attributesList; }
            set { this._attributesList = value; }
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

        public bool IsEditingAttributes
        {
            get { return this._isEditingAttributes; }
            set
            {
                this._isEditingAttributes = value;
                OnPropertyChanged("IsEditingAttributes");
            }
        }

        public string SelectedAttributes
        {
            get
            {
                string ret = "";

                foreach (MyAttribute ma in this.AttributesList)
                {
                    if (ma.CheckedStatus)
                    {
                        if (!ret.Equals(""))
                        {
                            ret += ", " + ma.Name;
                        }
                        else
                        {
                            ret += ma.Name;
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

        public bool TypeAttribute
        {
            get { return this.AttributesList[0].CheckedStatus; }
        }

        public bool StatusAttribute
        {
            get { return this.AttributesList[1].CheckedStatus; }
        }

        public bool CreatedAttribute
        {
            get { return this.AttributesList[2].CheckedStatus; }
        }

        public bool UpdatedAttribute
        {
            get { return this.AttributesList[3].CheckedStatus; }
        }

        public bool AssigneeAttribute
        {
            get { return this.AttributesList[4].CheckedStatus; }
        }

        public bool SummaryAttribute
        {
            get { return this.AttributesList[5].CheckedStatus; }
        }

        public bool PriorityAttribute
        {
            get { return this.AttributesList[6].CheckedStatus; }
        }

        public bool HasNext
        {
            get { return this._hasNext; }
            set
            {
                this._hasNext = value;
                OnPropertyChanged("HasNext");
            }
        }

        public bool HasPrevious
        {
            get { return this._hasPrevious; }
            set
            {
                this._hasPrevious = value;
                OnPropertyChanged("HasPrevious");
            }
        }

        public bool NoIssues
        {
            get { return this._noIssues; }
            set
            {
                this._noIssues = value;
                OnPropertyChanged("NoIssues");
            }
        }

        public bool NoIssuesSearch
        {
            get { return this._noIssuesSearch; }
            set
            {
                this._noIssuesSearch = value;
                OnPropertyChanged("NoIssuesSearch");
            }
        }
    }
}
