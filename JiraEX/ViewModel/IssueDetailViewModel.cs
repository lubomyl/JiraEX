using AtlassianConnector.Model.Exceptions;
using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.Controls;
using JiraEX.Main;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
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
    public class IssueDetailViewModel : ViewModelBase, ITitleable, IReinitializable
    {
        private const int TOTAL_NUMBER_OF_LOADINGS = 10;

        private bool _isEditingSummary = false;
        private bool _isEditingDescription = false;
        private bool _isEditingPriority = false;
        private bool _isEditingTransition = false;
        private bool _isEditingAssignee = false;
        private bool _isEditingFixVersions = false;
        private bool _isEditingAffectsVersion = false;
        private bool _isEditingLabels = false;
        private bool _isEditingSprint = false;
        private bool _isLinkingIssue = false;
        private bool _isEditingLinkedIssue = true;
        private bool _isEditingOriginalEstimate = false;

        private bool _isPriorityEditable = false;
        private bool _isSubTaskCreatable = false;
        private bool _isFixVersionsEditable = false;
        private bool _isAffectsVersionsEditable = false;
        private bool _haveSubtasks = false;
        private bool _haveAttachments = false;
        private bool _isDescriptionEmpty = true;
        private bool _isSprintEmpty = true;
        private bool _isLabelsEmpty = true;
        private bool _isFixVersionsEmpty = true;
        private bool _isAffectsVersionsEmpty = true;
        private bool _haveLinks = false;
        private bool _isSupportingSprints = false;

        private IJiraToolWindowNavigatorViewModel _parent;

        private IIssueService _issueService;
        private IPriorityService _priorityService;
        private ITransitionService _transitionService;
        private IAttachmentService _attachmentService;
        private IUserService _userService;
        private IBoardService _boardService;
        private IProjectService _projectService;

        private Issue _issue;
        private Project _project;
        private User _unassigned;

        private Priority _selectedPriority;
        private Transition _selectedTransition;
        private User _selectedAssignee;

        private Sprint _selectedSprint;
        private Issue _selectedLinkIssue;
        private IssueLinkTypeSplitted _selectedLinkType;

        private bool _isInitializingSprints = true;
        private bool _isInitializingAssignees = true;
        private bool _isRefreshing = false;

        private ObservableCollection<Priority> _priorityList;
        private ObservableCollection<Transition> _transitionList;
        private ObservableCollection<Attachment> _attachmentList;
        private ObservableCollection<User> _assigneeList;
        private ObservableCollection<JiraRESTClient.Model.Version> _fixVersionsList;
        private ObservableCollection<JiraRESTClient.Model.Version> _affectsVersionsList;
        private ObservableCollection<JiraRESTClient.Model.LabelSuggestion> _labelList;
        private ObservableCollection<Sprint> _sprintList;
        private ObservableCollection<Board> _boardList;
        private ObservableCollection<Issue> _issueList;
        private ObservableCollection<IssueLinkTypeSplitted> _issueLinkTypeList;
        private ObservableCollection<IssueLink> _inwardLinkedIssueList;
        private ObservableCollection<IssueLink> _outwardLinkedIssueList;

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

        public DelegateCommand CreateSubTaskCommand { get; private set; }

        public DelegateCommand ShowIssueCommand { get; private set; }

        public DelegateCommand EditAssigneeCommand { get; private set; }
        public DelegateCommand CancelEditAssigneeCommand { get; private set; }

        public DelegateCommand EditSprintCommand { get; private set; }
        public DelegateCommand CancelEditSprintCommand { get; private set; }

        public DelegateCommand EditFixVersionsCommand { get; private set; }
        public DelegateCommand CancelEditFixVersionsCommand { get; private set; }
        public DelegateCommand CheckedFixVersionCommand { get; private set; }

        public DelegateCommand EditAffectsVersionsCommand { get; private set; }
        public DelegateCommand CancelEditAffectsVersionsCommand { get; private set; }
        public DelegateCommand CheckedAffectsVersionCommand { get; private set; }

        public DelegateCommand EditLabelsCommand { get; private set; }
        public DelegateCommand CancelEditLabelsCommand { get; private set; }
        public DelegateCommand CheckedLabelsCommand { get; private set; }

        public DelegateCommand LinkIssueCommand { get; private set; }
        public DelegateCommand ConfirmLinkIssueCommand { get; private set; }
        public DelegateCommand CancelLinkIssueCommand { get; private set; }

        public DelegateCommand DeleteLinkedIssueCommand { get; private set; }

        public DelegateCommand OpenInBrowserCommand { get; private set; }

        public DelegateCommand EditLinkedIssueCommand { get; private set; }
        public DelegateCommand CancelEditLinkedIssueCommand { get; private set; }

        public DelegateCommand EditOriginalEstimateCommand { get; private set; }
        public DelegateCommand ConfirmEditOriginalEstimateCommand { get; private set; }
        public DelegateCommand CancelEditOriginalEstimateCommand { get; private set; }

        public DelegateCommand CreateWorklogCommand { get; private set; }

        private int _totalNumberOfLoadings = TOTAL_NUMBER_OF_LOADINGS;

        public IssueDetailViewModel(IJiraToolWindowNavigatorViewModel parent, Issue issue, Project project,
            IIssueService issueService, IPriorityService priorityService, ITransitionService transitionService,
            IAttachmentService attachmentService, IUserService userService, IBoardService boardService, IProjectService projectService)
        {
            this._parent = parent;
            this._parent.SetRefreshCommand(RefreshIssueDetails);

            Initialize(issueService, priorityService, transitionService,
            attachmentService, userService, boardService, projectService);

            this.Issue = issue;

            if (project == null)
            {
                this._project = this._issue.Fields.Project;
            }
            else
            {
                this._project = project;
            }

            GetIssuesAsync();
            GetIssueLinkTypesAsync();
            GetAssigneesAsync();
            GetLabelsAsync();
            GetBoardsAsync();
            GetCreatableIssueTypesAsync();
            GetEditablePropertiesAsync();
            GetPrioritiesAsync();
            GetTransitionsAsync();

            UpdateIssueAsync();

            SetPanelTitles();

            SeparateLinkedIssueTypes();
        }

        private void RefreshIssueDetails(object sender, EventArgs e)
        {
            this.ResetTotalNumberOfActiveLoadings();
            this._isRefreshing = true;

            this._parent.StartLoading();

            GetIssuesAsync();
            GetIssueLinkTypesAsync();
            GetAssigneesAsync();
            GetLabelsAsync();
            GetBoardsAsync();
            GetCreatableIssueTypesAsync();
            GetEditablePropertiesAsync();
            GetPrioritiesAsync();
            GetTransitionsAsync();

            UpdateIssueAsync();

            HideErrorMessages(this._parent);
        }

        private void CheckSubTaskCreatable()
        {
            foreach (IssueType it in this._project.CreatableIssueTypesList)
            {
                if (it.Subtask)
                {
                    this.IsSubTaskCreatable = true;
                    break;
                }
            }
        }

        private void CheckEmptyFields()
        {
            if (this._issue.Fields.Subtasks.Count != 0)
            {
                this.HaveSubtasks = true;
            } else
            {
                this.HaveSubtasks = false;
            }

            if (this._issue.Fields.Attachment.Count != 0)
            {
                this.HaveAttachments = true;
            }
            else
            {
                this.HaveAttachments = false;
            }

            if (this._issue.Fields.Description != null)
            {
                this.IsDescriptionEmpty = false;
            } else
            {
                this.IsDescriptionEmpty = true;
            }

            if (this._issue.Fields.Sprint != null) {
                this.IsSprintEmpty = false;
            } else
            {
                this.IsSprintEmpty = true;
            }

            if (this._issue.Fields.Labels.Count > 0)
            {
                this.IsLabelsEmpty = false;
            } else
            {
                this.IsLabelsEmpty = true;
            }

            if (this._issue.Fields.FixVersions.Count > 0)
            {
                this.IsFixVersionsEmpty = false;
            }
            else
            {
                this.IsFixVersionsEmpty = true;
            }

            if (this._issue.Fields.Versions.Count > 0)
            {
                this.IsAffectsVersionsEmpty = false;
            } else
            {
                this.IsAffectsVersionsEmpty = true;
            }
        }

        private void Initialize(IIssueService issueService, IPriorityService priorityService, ITransitionService transitionService,
            IAttachmentService attachmentService, IUserService userService, IBoardService boardService, IProjectService projectService)
        {
            this._unassigned = new User("Unassigned", "-1");

            this._issueService = issueService;
            this._priorityService = priorityService;
            this._transitionService = transitionService;
            this._attachmentService = attachmentService;
            this._userService = userService;
            this._boardService = boardService;
            this._projectService = projectService;

            this._priorityList = new ObservableCollection<Priority>();
            this._transitionList = new ObservableCollection<Transition>();
            this._attachmentList = new ObservableCollection<Attachment>();
            this._assigneeList = new ObservableCollection<User>();
            this._fixVersionsList = new ObservableCollection<JiraRESTClient.Model.Version>();
            this._affectsVersionsList = new ObservableCollection<JiraRESTClient.Model.Version>();
            this._labelList = new ObservableCollection<JiraRESTClient.Model.LabelSuggestion>();
            this._sprintList = new ObservableCollection<Sprint>();
            this._boardList = new ObservableCollection<Board>();
            this._issueList = new ObservableCollection<Issue>();
            this._issueLinkTypeList = new ObservableCollection<IssueLinkTypeSplitted>();
            this._inwardLinkedIssueList = new ObservableCollection<IssueLink>();
            this._outwardLinkedIssueList = new ObservableCollection<IssueLink>();

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

            this.ShowIssueCommand = new DelegateCommand(ShowIssue);

            this.EditAssigneeCommand = new DelegateCommand(EnableEditAssignee);
            this.CancelEditAssigneeCommand = new DelegateCommand(CancelEditAssignee);

            this.EditSprintCommand = new DelegateCommand(EnableEditSprint);
            this.CancelEditSprintCommand = new DelegateCommand(CancelEditSprint);

            this.EditFixVersionsCommand = new DelegateCommand(EnableEditFixVersions);
            this.CancelEditFixVersionsCommand = new DelegateCommand(CancelEditFixVersions);
            this.CheckedFixVersionCommand = new DelegateCommand(CheckedFixVersion);

            this.EditAffectsVersionsCommand = new DelegateCommand(EnableEditAffectsVersions);
            this.CancelEditAffectsVersionsCommand = new DelegateCommand(CancelEditAffectsVersions);
            this.CheckedAffectsVersionCommand = new DelegateCommand(CheckedAffectsVersion);

            this.EditLabelsCommand = new DelegateCommand(EnableEditLabels);
            this.CancelEditLabelsCommand = new DelegateCommand(CancelEditLabels);
            this.CheckedLabelsCommand = new DelegateCommand(CheckedLabel);

            this.LinkIssueCommand = new DelegateCommand(LinkIssue);
            this.ConfirmLinkIssueCommand = new DelegateCommand(ConfirmLinkIssue);
            this.CancelLinkIssueCommand = new DelegateCommand(CancelLinkIssue);

            this.DeleteLinkedIssueCommand = new DelegateCommand(DeleteLinkedIssue);

            this.OpenInBrowserCommand = new DelegateCommand(OpenInBrowser);

            this.EditLinkedIssueCommand = new DelegateCommand(EnableEditLinkedIssue);
            this.CancelEditLinkedIssueCommand = new DelegateCommand(CancelEditLinkedIssue);

            this.EditOriginalEstimateCommand = new DelegateCommand(EnableEditOriginalEstimate);
            this.ConfirmEditOriginalEstimateCommand = new DelegateCommand(ConfirmEditOriginalEstimate);
            this.CancelEditOriginalEstimateCommand = new DelegateCommand(CancelEditOriginalEstimate);

            this.CreateWorklogCommand = new DelegateCommand(CreateWorklog);
        }

        private void CreateWorklog(object obj)
        {
            JiraWorklogToolWindow tw = JiraPackage.JiraWorklogToolWindowVar;

            tw.ViewModel.OpenCreateWorklogForIssue(this.Issue);

            IVsWindowFrame windowFrame = (IVsWindowFrame)tw.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private async void ShowIssue(object sender)
        {
            this._parent.StartLoading();

            Issue issue = sender as Issue;

            try
            {
                var completeIssue = await this._issueService.GetIssueByIssueKeyAsync(issue.Key);

                this._parent.ShowIssueDetail(completeIssue, completeIssue.Fields.Project);

                HideErrorMessages(this._parent);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }
        }

        #region Checks on combobox

        private async void CheckedLabel(object sender)
        {
            this._parent.StartLoading();

            JiraRESTClient.Model.LabelSuggestion label = sender as JiraRESTClient.Model.LabelSuggestion;

            try
            {
                HideErrorMessages(this._parent);

                if (label.CheckedStatus)
                {
                    await this._issueService.UpdateIssuePropertyAsync(this.Issue.Key, "add", "labels", label.Label);
                }
                else
                {
                    await this._issueService.UpdateIssuePropertyAsync(this.Issue.Key, "remove", "labels", label.Label);
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private async void CheckedFixVersion(object sender)
        {
            this._parent.StartLoading();

            JiraRESTClient.Model.Version version = sender as JiraRESTClient.Model.Version;

            try
            {
                HideErrorMessages(this._parent);

                if (version.CheckedStatus)
                {
                    await this._issueService.AddIssueVersionPropertyAsync(this.Issue.Key, "fixVersions", version.Name);
                }
                else
                {
                    await this._issueService.RemoveIssueVersionPropertyAsync(this.Issue.Key, "fixVersions", version.Name);
                }

            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private async void CheckedAffectsVersion(object sender)
        {
            this._parent.StartLoading();

            JiraRESTClient.Model.Version version = sender as JiraRESTClient.Model.Version;

            try
            {
                HideErrorMessages(this._parent);

                if (version.CheckedStatus)
                {
                    await this._issueService.AddIssueVersionPropertyAsync(this.Issue.Key, "versions", version.Name);
                }
                else
                {
                    await this._issueService.RemoveIssueVersionPropertyAsync(this.Issue.Key, "versions", version.Name);
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        #endregion

        #region Async GET calls

        private async void GetCreatableIssueTypesAsync()
        {
            this._parent.StartLoading();

            try
            {
                var projectCreatableList = await this._projectService.GetAllProjectsCreatableIssueTypesAsync();

                foreach (ProjectCreatable pc in projectCreatableList.Projects)
                {
                    if (this._project.Id.Equals(pc.Id))
                    {
                        this._project.CreatableIssueTypesList = pc.Issuetypes;
                    }
                }

                CheckSubTaskCreatable();

            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetIssuesAsync()
        {
            this._parent.StartLoading();

            Task<IssueList> issueTask = this._issueService.GetAllIssues();

            try
            {
                var issueList = await issueTask as IssueList;
               
                this.IssueList.Clear();

                foreach (Issue i in issueList.Issues)
                {
                    if (this._issue.Id != i.Id)
                    {
                        this.IssueList.Add(i);
                        this.SelectedLinkIssue = i;
                    }
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetIssueLinkTypesAsync()
        {
            this._parent.StartLoading();

            Task<IssueLinkTypeList> issueLinkTypeTask = this._issueService.GetAllIssueLinkTypes();

            try
            {
                var issueLinkTypeList = await issueLinkTypeTask as IssueLinkTypeList;

                this.IssueList.Clear();
                this.IssueLinkTypesList.Clear();

                foreach (IssueLinkType i in issueLinkTypeList.IssueLinkTypes)
                {
                    if (!i.Inward.Equals(i.Outward))
                    {
                        IssueLinkTypeSplitted iltsInward = new IssueLinkTypeSplitted(i.Id, i.Name, i.Inward, "inward");
                        IssueLinkTypeSplitted iltsOutward = new IssueLinkTypeSplitted(i.Id, i.Name, i.Outward, "outward");

                        this.IssueLinkTypesList.Add(iltsInward);
                        this.IssueLinkTypesList.Add(iltsOutward);
                    }
                    else
                    {
                        IssueLinkTypeSplitted ilts = new IssueLinkTypeSplitted(i.Id, i.Name, i.Inward, null);

                        this.IssueLinkTypesList.Add(ilts);
                    }
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetPrioritiesAsync()
        {
            this._parent.StartLoading();

            Task<PriorityList> priorityTask = this._priorityService.GetAllPrioritiesAsync();

            try
            {
                var priorityList = await priorityTask as PriorityList;

                this.PriorityList.Clear();

                foreach (Priority p in priorityList)
                {
                    this.PriorityList.Add(p);

                    if (p.Id == this._issue.Fields.Priority.Id)
                    {
                        this.SelectedPriority = p;
                    }
                }

                HideErrorMessages(this._parent);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetTransitionsAsync()
        {
            this._parent.StartLoading();

            Task<TransitionList> transitionTask = this._transitionService.GetAllTransitionsForIssueByIssueKey(this._issue.Key);

            try
            {
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
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetAssigneesAsync()
        {
            this._parent.StartLoading();

            Task<UserList> userTask = this._userService.GetAllAssignableUsersForIssueByIssueKey(this._issue.Key);

            try
            {
                var userList = await userTask as UserList;

                this.AssigneeList.Clear();

                this.AssigneeList.Add(this._unassigned);

                foreach (User u in userList)
                {
                    this.AssigneeList.Add(u);

                    if (this._issue.Fields.Assignee != null)
                    {
                        if (u.Name == this._issue.Fields.Assignee.Name)
                        {
                            this.SelectedAssignee = u;
                        }
                    }
                }

                if (this.SelectedAssignee == null)
                {
                    this.SelectedAssignee = this.AssigneeList[0];
                }

                this._isInitializingAssignees = false;
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetBoardsAsync()
        {
            this._parent.StartLoading();

            Task<BoardList> boardsTask = this._boardService.GetAllBoardsByProjectKeyAsync(this._project.Key);

            try
            {
                var boardsList = await boardsTask as BoardList;

                this.BoardList.Clear();

                foreach (Board b in boardsList.Values)
                {
                    this.BoardList.Add(b);
                }

                GetSprintsAsync();
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetSprintsAsync()
        {
            this._parent.StartLoading();

            Task<SprintList> sprintsTask = this._boardService.GetAllSprintsByBoardIdAsync(this.BoardList[0].Id);

            try
            {
                var sprintsList = await sprintsTask as SprintList;

                this.SprintList.Clear();

                if (sprintsList != null && sprintsList.Values != null)
                {
                    this.IsSupportingSprints = true;

                    foreach (Sprint s in sprintsList.Values)
                    {
                        if (!s.State.Equals("closed"))
                        {
                            if (this.Issue.Fields.Sprint != null)
                            {
                                if (s.Id == this.Issue.Fields.Sprint.Id)
                                {
                                    this.SelectedSprint = s;
                                }
                            }

                            this.SprintList.Add(s);
                        }
                    }
                }

                this._isInitializingSprints = false;
            }
            catch (JiraException ex)
            {
                this.IsSupportingSprints = false;
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetLabelsAsync()
        {
            this._parent.StartLoading();

            Task<LabelsList> labelsTask = this._issueService.GetAllLabelsAsync("");

            try
            {
                var labelsList = await labelsTask as LabelsList;

                this.LabelsList.Clear();

                foreach (JiraRESTClient.Model.LabelSuggestion l in labelsList.Suggestions)
                {

                    foreach (string label in this.Issue.Fields.Labels)
                    {
                        if (label.Equals(l.Label))
                        {
                            l.CheckedStatus = true;
                            break;
                        }

                        l.CheckedStatus = false;
                    }

                    this.LabelsList.Add(l);
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void GetEditablePropertiesAsync()
        {
            this._parent.StartLoading();

            Task<EditableProperties> editablePropertiesTask = this._issueService.GetAllEditablePropertiesAsync(this._issue.Key);

            try { 
                var editableProperties = await editablePropertiesTask as EditableProperties;

                this._editablePropertiesFields = editableProperties.Fields;

                CheckEditableProperties();
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        #endregion

        #region Synchronous operations

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

        private void SeparateLinkedIssueTypes()
        {
            this.InwardLinkedIssueList.Clear();
            this.OutwardLinkedIssueList.Clear();

            foreach (IssueLink il in this._issue.Fields.IssueLinks)
            {
                if (il.InwardIssue != null)
                {
                    this.InwardLinkedIssueList.Add(il);
                } else
                {
                    this.OutwardLinkedIssueList.Add(il);
                }
            }

            if (this.InwardLinkedIssueList.Count > 0 || this.OutwardLinkedIssueList.Count > 0)
            {
                this.HaveLinks = true;
            }
            else
            {
                this.HaveLinks = false;
            }
        }

        private void FetchAffectsVersionsList()
        {
            this.AffectsVersionsList.Clear();

            foreach(JiraRESTClient.Model.Version v in this._editablePropertiesFields.Versions.AllowedValues)
            {
                foreach (JiraRESTClient.Model.Version ve in this.Issue.Fields.Versions)
                {
                    if (ve.Name.Equals(v.Name))
                    {
                        v.CheckedStatus = true;
                        break;
                    }

                    v.CheckedStatus = false;
                }
                this.AffectsVersionsList.Add(v);
            }
        }

        private void FetchFixVersionsList()
        {
            this.FixVersionsList.Clear();

            foreach (JiraRESTClient.Model.Version v in this._editablePropertiesFields.FixVersions.AllowedValues)
            {
                foreach(JiraRESTClient.Model.Version ve in this.Issue.Fields.FixVersions)
                {
                    if (ve.Name.Equals(v.Name))
                    {
                        v.CheckedStatus = true;
                        break;
                    }

                    v.CheckedStatus = false;
                }
                this.FixVersionsList.Add(v);
            }
        }

        #endregion

        #region Update calls

        private async void ConfirmEditSummary(object parameter)
        {
            this._parent.StartLoading();

            try
            {
                HideErrorMessages(this._parent);

                await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "set", "summary", this._issue.Fields.Summary);

                this.IsEditingSummary = false;
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private async void ConfirmEditDescription(object parameter)
        {
            this._parent.StartLoading();

            try
            {
                HideErrorMessages(this._parent);

                await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "set", "description", this._issue.Fields.Description);

                this.IsEditingDescription = false;
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private async void UpdatePriorityAsync()
        {
            this._parent.StartLoading();

            try
            {
                HideErrorMessages(this._parent);

                await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "set", "priority", this.SelectedPriority);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private void UpdateAttachments()
        {
            this.AttachmentsList.Clear();

            foreach (Attachment a in this.Issue.Fields.Attachment)
            {
                this.AttachmentsList.Add(a);
            }
        }

        private async void UpdateIssueAsync()
        {
            try
            {
                this.Issue = await this._issueService.GetIssueByIssueKeyAsync(this._issue.Key);
            
                CheckEmptyFields();
                UpdateAttachments();
                SeparateLinkedIssueTypes();
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.CheckTotalNumberOfActiveLoadings();
        }

        #endregion

        #region Operations with Issue

        private void CreateSubTask(object sender)
        {
            this._parent.StartLoading();

            this._parent.ShowCreateIssue(this._issue, this._project);
        }

        private async void DeleteLinkedIssue(object sender)
        {
            this._parent.StartLoading();

            IssueLink il = sender as IssueLink;

            try
            {
                HideErrorMessages(this._parent);

                await this._issueService.DeleteLinkedIssue(il.Id);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private void OpenInBrowser(object sender)
        {
            System.Diagnostics.Process.Start(UserSettingsHelper.ReadStringFromUserSettings("JiraBaseUrl") + "/browse/" + this.Issue.Key);
        }

        private async void ConfirmLinkIssue(object sender)
        {
            if (this._selectedLinkType != null && this._selectedLinkIssue != null)
            {
                this._parent.StartLoading();

                try
                {
                    HideErrorMessages(this._parent);

                    if (this._selectedLinkType.Type != null && this._selectedLinkType.Type.Equals("inward"))
                    {
                        await this._issueService.LinkIssue(this.SelectedLinkIssue.Key, this.Issue.Key, this._selectedLinkType.Name);
                    }
                    else
                    {
                        await this._issueService.LinkIssue(this.Issue.Key, this.SelectedLinkIssue.Key, this._selectedLinkType.Name);
                    }

                    this.IsLinkingIssue = false;
                }
                catch (JiraException ex)
                {
                    ShowErrorMessages(ex, this._parent);
                }
            }

            UpdateIssueAsync();
        }

        private async void DoTransitionAsync()
        {
            this._parent.StartLoading();

            try
            {
                HideErrorMessages(this._parent);

                await this._transitionService.DoTransitionAsync(this._issue.Key, this.SelectedTransition);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private async void AssignAsync()
        {
            this._parent.StartLoading();

            try
            {
                HideErrorMessages(this._parent);

                await this._issueService.AssignAsync(this._issue.Key, this._selectedAssignee.Name);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private async void UpdateSprint()
        {
            this._parent.StartLoading();

            try
            {
                HideErrorMessages(this._parent);

                await this._issueService.MoveIssueToSprintAsync(this.Issue.Key, this.SelectedSprint.Id);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        internal void DownloadAttachment(Attachment attachment)
        {
            this._parent.StartLoading();

            using (var client = new WebClient())
            {
                string downloadFolderPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",
                    "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                client.DownloadFileAsync(new System.Uri(attachment.Content), downloadFolderPath + "\\" + attachment.Filename);
                Process.Start("shell:Downloads");
            }

            this.DecrementTotalNumberOfActiveLoadings();
            this.CheckTotalNumberOfActiveLoadings();
        }

        private async void DeleteAttachment(object sender)
        {
            this._parent.StartLoading();

            Attachment attachment = sender as Attachment;

            try
            {
                HideErrorMessages(this._parent);

                await this._attachmentService.DeleteAttachmentByIdAsync(attachment.Id);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private async void UploadAttachmentFromFileBrowser(object sender)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Title = "Select attachment";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this._parent.StartLoading();

                string sSelectedPath = fileDialog.FileName;

                try {
                    this.IncrementTotalNumberOfActiveLoadings();

                    await this._issueService.PostAttachmentToIssueAsync(new FileInfo(sSelectedPath), this._issue.Key);

                    this.DecrementTotalNumberOfActiveLoadings();

                    UpdateIssueAsync();
                }
                catch (JiraException ex)
                {
                    this.DecrementTotalNumberOfActiveLoadings();
                    ShowErrorMessages(ex, this._parent);
                }  
            }
        }

        internal async void SearchAssignee(string searchString)
        {
            this._parent.StartLoading();

            Task<UserList> userTask = this._userService.GetAllAssignableUsersForIssueByIssueKeyAndByUsername(this._issue.Key, searchString);

            try
            {
                var userList = await userTask as UserList;

                this.AssigneeList.Clear();
                this.AssigneeList.Add(this._unassigned);

                foreach (User u in userList)
                {
                    this.AssigneeList.Add(u);
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this._parent.StopLoading();
            this.IsEditingAssignee = true;
        }


        internal async void SearchLabels(string searchString)
        {
            this._parent.StartLoading();

            Task<LabelsList> labelsTask = this._issueService.GetAllLabelsAsync(searchString);

            try
            {
                var labelsList = await labelsTask as LabelsList;

                this.LabelsList.Clear();

                foreach (JiraRESTClient.Model.LabelSuggestion l in labelsList.Suggestions)
                {

                    foreach (string label in this.Issue.Fields.Labels)
                    {
                        if (label.Equals(l.Label))
                        {
                            l.CheckedStatus = true;
                            break;
                        }

                        l.CheckedStatus = false;
                    }

                    this.LabelsList.Add(l);
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this._parent.StopLoading();
            this.IsEditingLabels = true;
        }

        internal async void SearchLinkedIssues(string searchString)
        {
            this._parent.StartLoading();

            Task<IssueList> issueTask = this._issueService.GetIssuesByIssueKeyAsync(searchString);

            try
            {
                var issueList = await issueTask as IssueList;

                this.IssueList.Clear();

                foreach (Issue i in issueList.Issues)
                {
                    if (this._issue.Id != i.Id)
                    {
                        this.IssueList.Add(i);
                    }
                }
            }
            catch (JiraException ex)
            {
                
            }

            this.IsEditingLinkedIssue = true;
            this._parent.StopLoading();
        }

        internal async void RefreshIssuesAsync()
        {
            this._parent.StartLoading();

            Task<IssueList> issueTask = this._issueService.GetAllIssues();

            try
            {
                var issueList = await issueTask as IssueList;

                this.IssueList.Clear();

                foreach (Issue i in issueList.Issues)
                {
                    if (this._issue.Id != i.Id)
                    {
                        this.IssueList.Add(i);
                    }
                }
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            this.IsEditingLinkedIssue = true;
            this._parent.StopLoading();
        }

        #endregion

        #region Visibility controls

        private void LinkIssue(object sender)
        {
            this.IsLinkingIssue = true;
        }

        private void CancelLinkIssue(object sender)
        {
            this.IsLinkingIssue = false;
        }

        private void EnableEditSummary(object parameter)
        {
            this.IsEditingSummary = true;
        }

        private void CancelEditSummary(object parameter)
        {
            this.IsEditingSummary = false;
        }

        private void EnableEditDescription(object parameter)
        {
            this.IsEditingDescription = true;
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

        private void EnableEditSprint(object parameter)
        {
            this.IsEditingSprint = true;
        }

        private void CancelEditSprint(object parameter)
        {
            this.IsEditingSprint = false;
        }

        private void EnableEditFixVersions(object parameter)
        {
            this.IsEditingFixVersions = true;
        }

        private void CancelEditFixVersions(object parameter)
        {
            this.IsEditingFixVersions = false;
        }

        private void EnableEditAffectsVersions(object parameter)
        {
            this.IsEditingAffectsVersions = true;
        }

        private void CancelEditAffectsVersions(object parameter)
        {
            this.IsEditingAffectsVersions = false;
        }

        private void EnableEditLabels(object parameter)
        {
            this.IsEditingLabels = true;
        }

        private void CancelEditLabels(object parameter)
        {
            this.IsEditingLabels = false;
        }

        private void EnableEditLinkedIssue(object parameter)
        {
            this.IsEditingLinkedIssue = true;
        }

        private void CancelEditLinkedIssue(object parameter)
        {
            if (this.SelectedLinkIssue == null)
            {
                this.IsEditingLinkedIssue = false;
            }
        }

        private void EnableEditOriginalEstimate(object parameter)
        {
            this.IsEditingOriginalEstimate = true;
        }

        private async void ConfirmEditOriginalEstimate(object parameter)
        {
            this._parent.StartLoading();

            try
            {
                HideErrorMessages(this._parent);

                await this._issueService.UpdateOriginalEstimatePropertyAsync(this._issue.Key, this.Issue.Fields.Timetracking.OriginalEstimate);

                this.IsEditingOriginalEstimate = false;
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);
            }

            UpdateIssueAsync();
        }

        private void CancelEditOriginalEstimate(object parameter)
        {
            this.IsEditingOriginalEstimate= false;
        }

        #endregion

        private void CheckTotalNumberOfActiveLoadings()
        {
            if (this._totalNumberOfLoadings <= 0)
            {
                this._parent.StopLoading();
                this._isRefreshing = false;
            }
        }     
        
        private void ResetTotalNumberOfActiveLoadings()
        {
            this._totalNumberOfLoadings += TOTAL_NUMBER_OF_LOADINGS;
        }

        private void IncrementTotalNumberOfActiveLoadings()
        {
            this._totalNumberOfLoadings += 1;
        }

        private void DecrementTotalNumberOfActiveLoadings()
        {
            this._totalNumberOfLoadings -= 1;
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

        public bool IsEditingAffectsVersions
        {
            get { return this._isEditingAffectsVersion; }
            set
            {
                this._isEditingAffectsVersion = value;
                OnPropertyChanged("IsEditingAffectsVersions");
            }
        }

        public bool IsEditingLabels
        {
            get { return this._isEditingLabels; }
            set
            {
                this._isEditingLabels = value;
                OnPropertyChanged("IsEditingLabels");
            }
        }

        public bool IsEditingSprint
        {
            get { return this._isEditingSprint; }
            set
            {
                this._isEditingSprint = value;
                OnPropertyChanged("IsEditingSprint");
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

        public void Reinitialize()
        {
            this._parent.SetRefreshCommand(RefreshIssueDetails);

            UpdateIssueAsync();
        }

        public bool IsLinkingIssue
        {
            get { return this._isLinkingIssue; }
            set
            {
                this._isLinkingIssue = value;
                OnPropertyChanged("IsLinkingIssue");
            }
        }

        public bool IsEditingLinkedIssue
        {
            get { return this._isEditingLinkedIssue; }
            set
            {
                this._isEditingLinkedIssue = value;
                OnPropertyChanged("IsEditingLinkedIssue");
            }
        }

        public bool IsEditingOriginalEstimate
        {
            get { return this._isEditingOriginalEstimate; }
            set
            {
                this._isEditingOriginalEstimate = value;
                OnPropertyChanged("IsEditingOriginalEstimate");
            }
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
            get { return this._attachmentList; }
            set
            {
                this._attachmentList = value;
                OnPropertyChanged("AttachmentsList");
            }
        }

        public Priority SelectedPriority
        {
            get { return this._selectedPriority; }
            set
            {
                if (this._selectedPriority != null && !this._isRefreshing)
                {
                    this._selectedPriority = value;
                    this.UpdatePriorityAsync();
                }
                else
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
                if (this._selectedTransition != null && !this._isRefreshing)
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

        public ObservableCollection<Board> BoardList
        {
            get { return this._boardList; }
            set
            {
                this._boardList = value;
                OnPropertyChanged("BoardList");
            }
        }

        public ObservableCollection<Sprint> SprintList
        {
            get { return this._sprintList; }
            set
            {
                this._sprintList = value;
                OnPropertyChanged("SprintList");
            }
        }

        public ObservableCollection<Issue> IssueList
        {
            get { return this._issueList; }
            set
            {
                this._issueList = value;
                OnPropertyChanged("IssueList");
            }
        }

        public User SelectedAssignee
        {
            get { return this._selectedAssignee; }
            set
            {
                if (!this._isInitializingAssignees && !this._isRefreshing)
                {
                    if (value != null)
                    {
                        this._selectedAssignee = value;
                        this.AssignAsync();
                    }
                }
                else
                {
                    this._selectedAssignee = value;
                }

                OnPropertyChanged("SelectedAssignee");
                this.IsEditingAssignee = false;
            }
        }

        public Sprint SelectedSprint
        {
            get { return this._selectedSprint; }
            set
            {
                if (!this._isInitializingSprints && !this._isRefreshing)
                {
                    this._selectedSprint = value;
                    this.UpdateSprint();
                }
                else
                {
                    this._selectedSprint = value;
                }
                
                OnPropertyChanged("SelectedSprint");

                this.IsEditingSprint = false;
            }
        }

        public Issue SelectedLinkIssue
        {
            get { return this._selectedLinkIssue; }
            set
            {
                if (value != null)
                {
                    this._selectedLinkIssue = value;
                    OnPropertyChanged("SelectedLinkIssue");
                }
            }
        }

        public IssueLinkTypeSplitted SelectedLinkType
        {
            get { return this._selectedLinkType; }
            set
            {
                this._selectedLinkType = value;
                OnPropertyChanged("SelectedLinkType");
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
                return this._issue.Fields.IssueType.Subtask;
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

        public bool HaveSubtasks
        {
            get { return this._haveSubtasks; }
            set
            {
                this._haveSubtasks = value;
                OnPropertyChanged("HaveSubtasks");
            }
        }

        public bool HaveAttachments
        {
            get { return this._haveAttachments; }
            set
            {
                this._haveAttachments = value;
                OnPropertyChanged("HaveAttachments");
            }
        }

        public bool HaveLinks
        {
            get { return this._haveLinks; }
            set
            {
                this._haveLinks = value;
                OnPropertyChanged("HaveLinks");
            }
        }

        public bool IsSupportingSprints
        {
            get { return this._isSupportingSprints; }
            set
            {
                this._isSupportingSprints = value;
                OnPropertyChanged("IsSupportingSprints");
            }
        }

        public bool IsDescriptionEmpty
        {
            get { return this._isDescriptionEmpty; }
            set
            {
                this._isDescriptionEmpty = value;
                OnPropertyChanged("IsDescriptionEmpty");
            }
        }

        public bool IsSprintEmpty
        {
            get { return this._isSprintEmpty; }
            set
            {
                this._isSprintEmpty = value;
                OnPropertyChanged("IsSprintEmpty");
            }
        }

        public bool IsLabelsEmpty
        {
            get { return this._isLabelsEmpty; }
            set
            {
                this._isLabelsEmpty = value;
                OnPropertyChanged("IsLabelsEmpty");
            }
        }

        public bool IsFixVersionsEmpty
        {
            get { return this._isFixVersionsEmpty; }
            set
            {
                this._isFixVersionsEmpty = value;
                OnPropertyChanged("IsFixVersionsEmpty");
            }
        }

        public bool IsAffectsVersionsEmpty
        {
            get { return this._isAffectsVersionsEmpty; }
            set
            {
                this._isAffectsVersionsEmpty = value;
                OnPropertyChanged("IsAffectsVersionsEmpty");
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

        public ObservableCollection<JiraRESTClient.Model.LabelSuggestion> LabelsList
        {
            get { return this._labelList; }
            set
            {
                this._labelList = value;
                OnPropertyChanged("LabelsList");
            }
        }

        public ObservableCollection<IssueLinkTypeSplitted> IssueLinkTypesList
        {
            get { return this._issueLinkTypeList; }
            set
            {
                this._issueLinkTypeList = value;
                OnPropertyChanged("IssueLinkTypesList");
            }
        }

        public ObservableCollection<IssueLink> InwardLinkedIssueList
        {
            get { return this._inwardLinkedIssueList; }
            set
            {
                this._inwardLinkedIssueList = value;
                OnPropertyChanged("InwardLinkedIssueList");
            }
        }

        public ObservableCollection<IssueLink> OutwardLinkedIssueList
        {
            get { return this._outwardLinkedIssueList; }
            set
            {
                this._outwardLinkedIssueList = value;
                OnPropertyChanged("OutwardLinkedIssueList");
            }
        }
    }
}
