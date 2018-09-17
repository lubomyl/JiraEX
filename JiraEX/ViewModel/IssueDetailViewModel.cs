using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiraEX.ViewModel
{
    public class IssueDetailViewModel : ViewModelBase, ITitleable
    {
        private bool _isEditingSummary = false;
        private bool _isEditingDescription = false;
        private bool _isEditingPriority = false;
        private bool _isEditingTransition = false;
        private bool _isEditingAssignee = false;
        private bool _isEditingFixVersions = false;

        private bool _isPriorityEditable = false;
        private bool _isSubTaskCreatable = false;
        private bool _isFixVersionsEditable = false;
        private bool _isAffectsVersionsEditable = false;

        private JiraToolWindowNavigatorViewModel _parent;

        private IIssueService _issueService;
        private IPriorityService _priorityService;
        private ITransitionService _transitionService;
        private IAttachmentService _attachmentService;
        private IUserService _userService;

        private Issue _issue;
        private BoardProject _project;

        private Priority _selectedPriority;
        private Transition _selectedTransition;
        private User _selectedAssignee;

        private ObservableCollection<Priority> _priorityList;
        private ObservableCollection<Transition> _transitionList;
        private ObservableCollection<Attachment> _attachmentsList;
        private ObservableCollection<User> _assigneeList;
        private ObservableCollection<JiraRESTClient.Model.Version> _fixVersionsList;
        private ObservableCollection<JiraRESTClient.Model.Version> _affectsVersionsList;

        private EditablePropertiesFields _editablePropertiesFields;

        public DelegateCommand EditSummaryCommand { get; private set; }
        public DelegateCommand ConfirmEditSummaryCommand { get; private set; }
        public DelegateCommand CancelEditSummaryCommand { get; private set; }

        public DelegateCommand EditDescriptionCommand { get; private set; }
        public DelegateCommand ConfirmEditDescriptionCommand { get; private set; }
        public DelegateCommand CancelEditDescriptionCommand { get; private set; }

        public DelegateCommand EditPriorityCommand { get; private set; }
        public DelegateCommand CancelEditPriorityCommand { get; private set; }

        public DelegateCommand EditTransitionCommand { get; private set; }
        public DelegateCommand CancelEditTransitionCommand { get; private set; }

        public DelegateCommand SelectFileToUploadCommand { get; private set; }

        public DelegateCommand DeleteAttachmentCommand { get; private set; }

        public DelegateCommand CreateSubTaskCommand { get; set; }

        public DelegateCommand ShowParentIssueCommand { get; set; }

        public DelegateCommand EditAssigneeCommand { get; set; }
        public DelegateCommand CancelEditAssigneeCommand { get; set; }

        public DelegateCommand EditFixVersionsCommand { get; set; }
        public DelegateCommand CancelEditFixVersionsCommand { get; set; }
        public DelegateCommand CheckedFixVersionCommand { get; set; }

        public IssueDetailViewModel(JiraToolWindowNavigatorViewModel parent, Issue issue, BoardProject project)
        {
            this._parent = parent;

            Initialize();

            this.Issue = issue;
            this._project = project;

            GetPrioritiesAsync();
            GetTransitionsAsync();
            GetEditablePropertiesAsync();
            CheckSubTaskCreatable();
            GetAssigneesAsync();

            UpdateIssueAsync();

            SetPanelTitles();
        }

        private void CheckSubTaskCreatable()
        {
            foreach(IssueType it in this._project.CreatableIssueTypesList)
            {
                if (it.Subtask)
                {
                    this.IsSubTaskCreatable = true;
                    break;
                }
            }
        }

        private void Initialize()
        {
            this._issueService = new IssueService();
            this._priorityService = new PriorityService();
            this._transitionService = new TransitionService();
            this._attachmentService = new AttachmentService();
            this._userService = new UserService();

            this._priorityList = new ObservableCollection<Priority>();
            this._transitionList = new ObservableCollection<Transition>();
            this._attachmentsList = new ObservableCollection<Attachment>();
            this._assigneeList = new ObservableCollection<User>();
            this._fixVersionsList = new ObservableCollection<JiraRESTClient.Model.Version>();
            this._affectsVersionsList = new ObservableCollection<JiraRESTClient.Model.Version>();

            this.EditSummaryCommand = new DelegateCommand(EnableEditSummary);
            this.ConfirmEditSummaryCommand = new DelegateCommand(ConfirmEditSummary);
            this.CancelEditSummaryCommand = new DelegateCommand(CancelEditSummary);

            this.EditDescriptionCommand = new DelegateCommand(EnableEditDescription);
            this.ConfirmEditDescriptionCommand = new DelegateCommand(ConfirmEditDescription);
            this.CancelEditDescriptionCommand = new DelegateCommand(CancelEditDescription);

            this.EditPriorityCommand = new DelegateCommand(EnableEditPriority);
            this.CancelEditPriorityCommand = new DelegateCommand(CancelEditPriority);

            this.EditTransitionCommand = new DelegateCommand(EnableEditTransition);
            this.CancelEditTransitionCommand = new DelegateCommand(CancelEditTransition);

            this.SelectFileToUploadCommand = new DelegateCommand(UploadAttachmentFromFileBrowser);

            this.DeleteAttachmentCommand = new DelegateCommand(DeleteAttachment);

            this.CreateSubTaskCommand = new DelegateCommand(CreateSubTask);

            this.ShowParentIssueCommand = new DelegateCommand(ShowParentIssue);

            this.EditAssigneeCommand = new DelegateCommand(EnableEditAssignee);
            this.CancelEditAssigneeCommand = new DelegateCommand(CancelEditAssignee);

            this.EditFixVersionsCommand = new DelegateCommand(EnableEditFixVersions);
            this.CancelEditFixVersionsCommand = new DelegateCommand(CancelEditFixVersions);
            this.CheckedFixVersionCommand = new DelegateCommand(CheckedFixVersion);
        }

        private async void CheckedFixVersion(object sender)
        {
            JiraRESTClient.Model.Version version = sender as JiraRESTClient.Model.Version;

            if (version.CheckedStatus) {
                await this._issueService.AddIssueVersionPropertyAsync(this.Issue.Key, "fixVersions", version.Name);
            } else
            {
                await this._issueService.RemoveIssueVersionPropertyAsync(this.Issue.Key, "fixVersions", version.Name);
            }
        }

        private async void ShowParentIssue(object sender)
        {
            var completeIssue = await this._issueService.GetIssueByIssueKeyAsync(this._issue.Fields.Parent.Key);

            this._parent.ShowIssueDetail(completeIssue, this._project);
        }

        private void CreateSubTask(object sender)
        {
            this._parent.CreateIssue(this._issue, this._project);
        }

        internal void DownloadAttachment(Attachment attachment)
        {
            using (var client = new WebClient())
            {
                string downloadFolderPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", 
                    "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                client.DownloadFileAsync(new System.Uri(attachment.Content), downloadFolderPath + "\\" + attachment.Filename);
                Process.Start("shell:Downloads");
            }
        }

        private async void DeleteAttachment(object sender)
        {
            Attachment attachment = sender as Attachment;

            await this._attachmentService.DeleteAttachmentByIdAsync(attachment.Id);

            UpdateIssueAsync();
        }

        private async void UploadAttachmentFromFileBrowser(object sender)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Title = "Select attachment";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fileDialog.FileName;
                await this._issueService.PostAttachmentToIssueAsync(new FileInfo(sSelectedPath), this._issue.Key);
            }

            UpdateIssueAsync();
        }

        private async void GetPrioritiesAsync()
        {
            System.Threading.Tasks.Task<PriorityList> priorityTask = this._priorityService.GetAllPrioritiesAsync();

            var priorityList = await priorityTask as PriorityList;

            this.PriorityList.Clear();

            foreach (Priority p in priorityList)
            {
                this.PriorityList.Add(p);

                if(p.Id == this._issue.Fields.Priority.Id)
                {
                    this.SelectedPriority = p;
                }
            }
        }

        private async void GetTransitionsAsync()
        {
            System.Threading.Tasks.Task<TransitionList> transitionTask = this._transitionService.GetAllTransitionsForIssueByIssueKey(this._issue.Key);

            var transitionList = await transitionTask as TransitionList;

            this.TransitionList.Clear();

            foreach (Transition t in transitionList.Transitions)
            {
                this.TransitionList.Add(t);

                if (t.Name == this._issue.Fields.Status.Name)
                {
                    this.SelectedTransition = t;
                }
            }
        }

        private async void GetAssigneesAsync()
        {
            System.Threading.Tasks.Task<UserList> userTask = this._userService.GetAllAssignableUsersForIssueByIssueKey(this._issue.Key);

            var userList = await userTask as UserList;

            this.AssigneeList.Clear();

            foreach (User u in userList)
            {
                this.AssigneeList.Add(u);

                if (u.Name == this._issue.Fields.Status.Name)
                {
                    this.SelectedAssignee = u;
                }
            }
        }

        private async void GetEditablePropertiesAsync()
        {
            System.Threading.Tasks.Task<EditableProperties> editablePropertiesTask = this._issueService.GetAllEditablePropertiesAsync(this._issue.Key);

            var editableProperties = await editablePropertiesTask as EditableProperties;

            this._editablePropertiesFields = editableProperties.Fields;

            CheckEditableProperties();
        }

        private void CheckEditableProperties()
        {
            this.IsPriorityEditable = IsPropertyEditable("priority");
            this.IsAffectsVersionsEditable = IsPropertyEditable("versions");
            this.IsFixVersionsEditable = IsPropertyEditable("fixVersions");

            if (this.IsAffectsVersionsEditable)
            {
                FetchAffectsVersionsList();
            }

            if (this.IsFixVersionsEditable)
            {
                FetchFixVersionsList();
            }
        }

        private void FetchAffectsVersionsList()
        {
            this.AffectsVersionsList.Clear();

            foreach(JiraRESTClient.Model.Version v in this._editablePropertiesFields.Versions.AllowedValues)
            {
                this.AffectsVersionsList.Add(v);
            }
        }

        private void FetchFixVersionsList()
        {
            this.FixVersionsList.Clear();

            foreach (JiraRESTClient.Model.Version v in this._editablePropertiesFields.FixVersions.AllowedValues)
            {
                v.CheckedStatus = false;
                this.FixVersionsList.Add(v);
            }
        }

        private async void UpdatePriorityAsync()
        {
            await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "priority", this.SelectedPriority);

            UpdateIssueAsync();
        }

        private async void UpdateIssueAsync()
        {
            this.Issue = await this._issueService.GetIssueByIssueKeyAsync(this._issue.Key);

            UpdateAttachments();
        }

        private void UpdateAttachments()
        {
            this.AttachmentsList.Clear();

            foreach (Attachment a in this.Issue.Fields.Attachment)
            {
                this.AttachmentsList.Add(a);
            }
        }

        private async void DoTransitionAsync()
        {
            await this._transitionService.DoTransitionAsync(this._issue.Key, this.SelectedTransition);

            UpdateIssueAsync();
        }

        private async void AssignAsync()
        {
            await this._issueService.AssignAsync(this._issue.Key, this._selectedAssignee.Name);

            UpdateIssueAsync();
        }

        private void EnableEditSummary(object parameter)
        {
            this.IsEditingSummary = true;
        }

        private async void ConfirmEditSummary(object parameter)
        {
            await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "summary", this._issue.Fields.Summary);

            UpdateIssueAsync();

            this.IsEditingSummary = false;
        }

        private void CancelEditSummary(object parameter)
        {
            this.IsEditingSummary = false;
        }

        private void EnableEditDescription(object parameter)
        {
            this.IsEditingDescription = true;
        }

        private async void ConfirmEditDescription(object parameter)
        {
            await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "description", this._issue.Fields.Description);

            UpdateIssueAsync();

            this.IsEditingDescription = false;
        }

        private void CancelEditDescription(object parameter)
        {
            this.IsEditingDescription = false;
        }

        private void EnableEditPriority(object parameter)
        {
            this.IsEditingPriority = true;
        }

        private void CancelEditPriority(object parameter)
        {
            this.IsEditingPriority = false;
        }

        private void EnableEditTransition(object parameter)
        {
            this.IsEditingTransition = true;
        }

        private void CancelEditTransition(object parameter)
        {
            this.IsEditingTransition = false;
        }

        private void EnableEditAssignee(object parameter)
        {
            this.IsEditingAssignee = true;
        }

        private void CancelEditAssignee(object parameter)
        {
            this.IsEditingAssignee = false;
        }

        private void EnableEditFixVersions(object parameter)
        {
            this.IsEditingFixVersions = true;
        }

        private void CancelEditFixVersions(object parameter)
        {
            this.IsEditingFixVersions = false;
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("Issue " + this.Issue.Key, Issue.Fields.Project.Name);
        }

        public bool IsEditingSummary
        {
            get { return this._isEditingSummary; }
            set
            {
                this._isEditingSummary = value;
                OnPropertyChanged("IsEditingSummary");
            }
        }

        public bool IsEditingDescription
        {
            get { return this._isEditingDescription; }
            set
            {
                this._isEditingDescription = value;
                OnPropertyChanged("IsEditingDescription");
            }
        }

        public bool IsEditingPriority
        {
            get { return this._isEditingPriority; }
            set
            {
                this._isEditingPriority = value;
                OnPropertyChanged("IsEditingPriority");
            }
        }

        public bool IsEditingTransition
        {
            get { return this._isEditingTransition; }
            set
            {
                this._isEditingTransition = value;
                OnPropertyChanged("IsEditingTransition");
            }
        }

        public bool IsEditingAssignee
        {
            get { return this._isEditingAssignee; }
            set {
                this._isEditingAssignee = value;
                OnPropertyChanged("IsEditingAssignee");
            }
        }

        public bool IsEditingFixVersions
        {
            get { return this._isEditingFixVersions; }
            set
            {
                this._isEditingFixVersions = value;
                OnPropertyChanged("IsEditingFixVersions");
            }
        }

        private bool IsPropertyEditable(string propertyName)
        {
            bool ret = false;

            if (propertyName.Equals("priority"))
            {
                ret = this._editablePropertiesFields.Priority != null;
            } else if (propertyName.Equals("versions"))
            {
                ret = this._editablePropertiesFields.Versions != null;
            } else if (propertyName.Equals("fixVersions"))
            {
                ret = this._editablePropertiesFields.FixVersions != null;
            }

            return ret;
        }

        public Issue Issue
        {
            get { return this._issue; }
            set
            {
                this._issue = value;
                OnPropertyChanged("Issue");
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

        public ObservableCollection<Attachment> AttachmentsList
        {
            get { return this._attachmentsList; }
            set
            {
                this._attachmentsList = value;
                OnPropertyChanged("AttachmentsList");
            }
        }

        public Priority SelectedPriority
        {
            get { return this._selectedPriority; }
            set
            {
                if (this._selectedPriority != null)
                {
                    this._selectedPriority = value;
                    this.UpdatePriorityAsync();
                }
                else if (this._selectedPriority == null)
                {
                    this._selectedPriority = value;
                }
  
                OnPropertyChanged("SelectedPriority");
                this.IsEditingPriority = false;
            }
        }

        public ObservableCollection<Transition> TransitionList
        {
            get { return this._transitionList; }
            set
            {
                this._transitionList = value;
                OnPropertyChanged("TransitionList");
            }
        }

        public Transition SelectedTransition
        {
            get { return this._selectedTransition; }
            set
            {
                if (this._selectedTransition != null)
                {
                    this._selectedTransition = value;
                    this.DoTransitionAsync();
                }
                else
                {
                    this._selectedTransition = value;
                }

                OnPropertyChanged("SelectedTransition");
                this.IsEditingTransition = false;
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

        public User SelectedAssignee
        {
            get { return this._selectedAssignee; }
            set
            {
                if (this._selectedAssignee != null)
                {
                    this._selectedAssignee = value;
                    this.AssignAsync();
                }
                else
                {
                    this._selectedAssignee = value;
                }

                OnPropertyChanged("SelectedAssignee");
                this.IsEditingAssignee = false;
            }
        }

        public bool IsPriorityEditable
        {
            get { return this._isPriorityEditable; }
            set
            {
                this._isPriorityEditable = value;
                OnPropertyChanged("IsPriorityEditable");
            }
        }

        public bool IsSubTaskCreatable
        {
            get { return this._isSubTaskCreatable; }
            set
            {
                this._isSubTaskCreatable = value;
                OnPropertyChanged("IsSubTaskCreatable");
            }
        }

        public bool IsSubTask
        {
            get
            {
                return this._issue.Fields.Issuetype.Subtask;
            }
            set
            {
                OnPropertyChanged("IsSubTask");
            }
        }

        public bool IsFixVersionsEditable
        {
            get { return this._isFixVersionsEditable; }
            set
            {
                this._isFixVersionsEditable = value;
                OnPropertyChanged("IsFixVersionsEditable");
            }
        }

        public bool IsAffectsVersionsEditable
        {
            get { return this._isAffectsVersionsEditable; }
            set
            {
                this._isAffectsVersionsEditable = value;
                OnPropertyChanged("IsAffectsVersionsEditable");
            }
        }

        public ObservableCollection<JiraRESTClient.Model.Version> FixVersionsList
        {
            get { return this._fixVersionsList; }
            set
            {
                this._fixVersionsList = value;
                OnPropertyChanged("FixVersionsList");
            }
        }

        public ObservableCollection<JiraRESTClient.Model.Version> AffectsVersionsList
        {
            get { return this._affectsVersionsList; }
            set
            {
                this._affectsVersionsList = value;
                OnPropertyChanged("AffectsVersionsList");
            }
        }

    }
}
