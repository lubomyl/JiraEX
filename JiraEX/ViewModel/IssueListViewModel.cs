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

        private bool _isEditingAttributes;

        private ObservableCollection<Issue> _issueList;
        private ObservableCollection<MyAttribute> _attributesList;

        private string _subtitle;
        private bool _canCreateIssue = false;

        public DelegateCommand CreateIssueCommand { get; private set; }

        public DelegateCommand CheckedAttributeCommand { get; set; }
        public DelegateCommand EditAttributesCommand { get; set; }
        public DelegateCommand CancelEditAttributesCommand { get; set; }

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

            InitializeAttributes();

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
                    break;
                case "Status":
                    OnPropertyChanged("StatusAttribute");
                    break;
                case "Created":
                    OnPropertyChanged("CreatedAttribute");
                    break;
                case "Updated":
                    OnPropertyChanged("UpdatedAttribute");
                    break;
                case "Assignee":
                    OnPropertyChanged("AssigneeAttribute");
                    break;
                case "Summary":
                    OnPropertyChanged("SummaryAttribute");
                    break;
                case "Priority":
                    OnPropertyChanged("PriorityAttribute");
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
            this.AttributesList.Add(new MyAttribute("Type", true));
            this.AttributesList.Add(new MyAttribute("Status", true));
            this.AttributesList.Add(new MyAttribute("Created", false));
            this.AttributesList.Add(new MyAttribute("Updated", false));
            this.AttributesList.Add(new MyAttribute("Assignee", false));
            this.AttributesList.Add(new MyAttribute("Summary", true));
            this.AttributesList.Add(new MyAttribute("Priority", true));
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
    }
}
