using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        private bool _isPriorityEditable = false;

        private JiraToolWindowNavigatorViewModel _parent;

        private IIssueService _issueService;
        private IPriorityService _priorityService;
        private ITransitionService _transitionService;

        private Issue _issue;

        private Priority _selectedPriority;
        private Transition _selectedTransition;

        private ObservableCollection<Priority> _priorityList;
        private ObservableCollection<Transition> _transitionList;
        private ObservableCollection<Attachment> _attachmentsList;

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

        public IssueDetailViewModel(JiraToolWindowNavigatorViewModel parent, Issue issue)
        {
            this._parent = parent;

            Initialize();

            this.Issue = issue;

            GetPrioritiesAsync();
            GetTransitionsAsync();
            GetEditablePropertiesAsync();

            SetPanelTitles();
        }

        private void Initialize()
        {
            this._issueService = new IssueService();
            this._priorityService = new PriorityService();
            this._transitionService = new TransitionService();

            this._priorityList = new ObservableCollection<Priority>();
            this._transitionList = new ObservableCollection<Transition>();
            this._attachmentsList = new ObservableCollection<Attachment>();

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
        }

        private async void UploadAttachmentFromFileBrowser(object sender)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
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

        private bool IsPropertyEditable(string propertyName)
        {
            bool ret = false;

            if (propertyName.Equals("priority"))
            {
                ret = this._editablePropertiesFields.Priority != null;
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
                this._selectedPriority = value;

                if (this._selectedPriority != null)
                {
                    this.UpdatePriorityAsync();
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
                this._selectedTransition = value;

                if (this._selectedTransition != null)
                {
                    this.DoTransitionAsync();
                }

                OnPropertyChanged("SelectedTransition");
                this.IsEditingTransition = false;
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
    }
}
