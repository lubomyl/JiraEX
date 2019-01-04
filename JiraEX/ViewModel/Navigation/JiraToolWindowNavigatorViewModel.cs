using ConfluenceEX.Helper;
using ConfluenceEX.ViewModel.Navigation;
using DevDefined.OAuth.Framework;
using JiraEX.Main;
using JiraEX.View;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JiraEX.ViewModel.Navigation
{
    public class JiraToolWindowNavigatorViewModel : ViewModelBase, IJiraToolWindowNavigatorViewModel
    {

        private object _selectedView;
        private JiraToolWindow _parent;

        private IVsWindowSearchHost _searchHost;

        private IOAuthService _oAuthService;
        private IBasicService _basicService;

        private IUserService _userService;
        private IIssueService _issueService;
        private ITransitionService _transitionService;
        private IPriorityService _priorityService;
        private IAttachmentService _attachmentService;
        private IBoardService _boardService;
        private ISprintService _sprintService;
        private IProjectService _projectService;

        private AuthenticationView _authenticationView;
        private AuthenticationVerificationView _authenticationVerification;
        private ConnectionView _connectionView;
        private ProjectListView _projectListView;
        private IssueListView _issueListView;
        private IssueDetailView _issueDetailView;
        private CreateIssueView _createIssueView;
        private FiltersListView _filtersListView;
        private AdvancedSearchView _advancedSearchView;
        private NoIssueFoundView _noIssueFoundView;

        private CommandID toolbarMenuCommandRefreshID;

        private HistoryNavigator _historyNavigator;

        private OleMenuCommandService _service;

        private string _title;
        private string _subtitle;

        private string _errorMessage;

        private bool _isLoading;

        public JiraToolWindowNavigatorViewModel(JiraToolWindow parent)
        {
            this._parent = parent;

            this._searchHost = parent.SearchHost;

            this._historyNavigator = new HistoryNavigator();
            
            this._service = JiraPackage.Mcs;

            this._oAuthService = new OAuthService();
            this._basicService = new BasicService();
            
            InitializeCommands(this._service);
        }

        public void InitializeServicesWithAuthenticationType(AuthenticationType type)
        {
            if(type == AuthenticationType.Base)
            {
                this._userService = new UserService(AuthenticationType.Base);
                this._issueService = new IssueService(AuthenticationType.Base);
                this._transitionService = new TransitionService(AuthenticationType.Base);
                this._priorityService = new PriorityService(AuthenticationType.Base);
                this._attachmentService = new AttachmentService(AuthenticationType.Base);
                this._boardService = new BoardService(AuthenticationType.Base);
                this._sprintService = new SprintService(AuthenticationType.Base);
                this._projectService = new ProjectService(AuthenticationType.Base);
            }
            else if (type == AuthenticationType.OAuth)
            {
                this._userService = new UserService(AuthenticationType.OAuth);
                this._issueService = new IssueService(AuthenticationType.OAuth);
                this._transitionService = new TransitionService(AuthenticationType.OAuth);
                this._priorityService = new PriorityService(AuthenticationType.OAuth);
                this._attachmentService = new AttachmentService(AuthenticationType.OAuth);
                this._boardService = new BoardService(AuthenticationType.OAuth);
                this._sprintService = new SprintService(AuthenticationType.OAuth);
                this._projectService = new ProjectService(AuthenticationType.OAuth);
            }
        }

        public void SignOut(object sender, EventArgs e)
        {
            ShowSignOutDialog();
        }

        protected void ShowSignOutDialog()
        {
            MessageBox_Show(ProcessCancelCreateIssueDialogSelection, "Are you sure want to sign out?", "Alert", MessageBoxButton.YesNo);
        }

        public void ProcessCancelCreateIssueDialogSelection(MessageBoxResult result)
        {
            if (result == MessageBoxResult.Yes)
            {
                UserSettingsHelper.DeletePropertyFromUserSettings("JiraAccessToken");
                UserSettingsHelper.DeletePropertyFromUserSettings("JiraAccessTokenSecret");
                UserSettingsHelper.DeletePropertyFromUserSettings("JiraBaseUrl");

                this.ShowAuthentication();
            }
        }

        public void TryToSignIn()
        {
            this.StopLoading();

            try
            {
                string accessToken = UserSettingsHelper.ReadStringFromUserSettings("JiraAccessToken");
                string accessTokenSecret = UserSettingsHelper.ReadStringFromUserSettings("JiraAccessTokenSecret");
                string baseUrl = UserSettingsHelper.ReadStringFromUserSettings("JiraBaseUrl");

                if (accessToken != null && accessTokenSecret != null && baseUrl != null)
                {
                    this._oAuthService.ReinitializeOAuthSessionAccessToken(accessToken, accessTokenSecret, baseUrl);

                    this.ShowProjects(null, null);
                } else
                {
                    this.ShowAuthentication();
                }
            }
            catch (Exception ex)
            {
                this.ShowAuthentication();
            }
        }

        public void ShowAuthentication()
        {
            this.StopLoading();

            this.EnableCommand(false, this._service, Guids.COMMAND_REFRESH_ID);
            this.EnableCommand(false, this._service, Guids.COMMAND_HOME_ID);
            this.EnableCommand(false, this._service, Guids.COMMAND_FILTERS_ID);
            this.EnableCommand(false, this._service, Guids.COMMAND_ADVANCED_SEARCH_ID);
            this.EnableCommand(false, this._service, Guids.COMMAND_CONNECTION_ID);

            this._historyNavigator.ClearStack();

            if (this._authenticationView == null)
            {
                this._authenticationView = new AuthenticationView(this, this._oAuthService);

                SelectedView = this._authenticationView;
            }
            else
            {
                SelectedView = _authenticationView;
            }
        }

        public void ShowAuthenticationVerification(object sender, EventArgs e, IToken requestToken)
        {
            this.StopLoading();
            this.EnableCommand(false, this._service, Guids.COMMAND_REFRESH_ID);

            this._authenticationVerification = new AuthenticationVerificationView(this, requestToken, this._oAuthService);

            SelectedView = this._authenticationVerification;
        }

        public void ShowProjects(object sender, EventArgs e)
        {
            this.StopLoading();
            this.EnableCommand(true, this._service, Guids.COMMAND_REFRESH_ID);
            this.EnableCommand(true, this._service, Guids.COMMAND_CONNECTION_ID);

            if (this._projectListView == null)
            {
                this._projectListView = new ProjectListView(this, this._projectService, this._boardService);
                this._historyNavigator.AddView(this._projectListView);

                SelectedView = this._projectListView;
            }
            else
            {
                this._historyNavigator.AddView(this._projectListView);

                SelectedView = this._projectListView;
            }
        }

        public void ShowIssuesOfProject(Project project)
        {
            this.StopLoading();
            this.EnableCommand(true, this._service, Guids.COMMAND_REFRESH_ID);

            this._issueListView = new IssueListView(this, this._issueService, project);
            this._historyNavigator.AddView(this._issueListView);

            SelectedView = this._issueListView;
        }

        public void ShowIssuesOfFilter(Filter filter)
        {
            this.StopLoading();
            this.EnableCommand(true, this._service, Guids.COMMAND_REFRESH_ID);

            this._issueListView = new IssueListView(this, this._issueService, this._sprintService, filter.Jql);
            this._historyNavigator.AddView(this._issueListView);

            SelectedView = this._issueListView;
        }

        public void ShowIssuesQuickSearch(string searchString)
        {
            this.StopLoading();
            this.EnableCommand(true, this._service, Guids.COMMAND_REFRESH_ID);

            this._issueListView = new IssueListView(this, this._issueService, this._sprintService, true, searchString);
            this._historyNavigator.AddView(this._issueListView);

            SelectedView = this._issueListView;
        }

        public void ShowIssueDetail(Issue issue, Project project)
        {
            this.StopLoading();
            this.EnableCommand(true, this._service, Guids.COMMAND_REFRESH_ID);

            this._issueDetailView = new IssueDetailView(this, issue, project, 
                this._issueService, this._priorityService, this._transitionService,
                this._attachmentService, this._userService, this._boardService, this._projectService);
            this._historyNavigator.AddView(this._issueDetailView);

            SelectedView = this._issueDetailView;
        }

        public void ShowCreateIssue(Project project)
        {
            this.StopLoading();
            this.EnableCommand(false, this._service, Guids.COMMAND_REFRESH_ID);

            this._createIssueView = new CreateIssueView(this, project, this._issueService);
            this._historyNavigator.AddView(this._createIssueView);

            SelectedView = this._createIssueView;
        }

        //sub-task overload
        public void ShowCreateIssue(Issue parentIssue, Project project)
        {
            this.StopLoading();
            this.EnableCommand(false, this._service, Guids.COMMAND_REFRESH_ID);

            this._createIssueView = new CreateIssueView(this, parentIssue, project, this._issueService);
            this._historyNavigator.AddView(this._createIssueView);

            SelectedView = this._createIssueView;
        }

        public void ShowFilters(object sender, EventArgs e)
        {
            this.StopLoading();
            this.EnableCommand(true, this._service, Guids.COMMAND_REFRESH_ID);

            this._filtersListView = new FiltersListView(this, this._issueService);
            this._historyNavigator.AddView(this._filtersListView);

            SelectedView = this._filtersListView;
        }

        public void ShowAdvancedSearch(object sender, EventArgs e)
        {
            this.StopLoading();
            this.EnableCommand(false, this._service, Guids.COMMAND_REFRESH_ID);

            this._advancedSearchView = new AdvancedSearchView(this, this._priorityService, this._issueService, this._projectService,
                this._sprintService, this._boardService);
            this._historyNavigator.AddView(this._advancedSearchView);

            SelectedView = this._advancedSearchView;
        }

        public void ShowNoIssueFound(string issueKey)
        {
            this.StopLoading();
            this.EnableCommand(false, this._service, Guids.COMMAND_REFRESH_ID);

            this._noIssueFoundView = new NoIssueFoundView(this, issueKey);
            this._historyNavigator.AddView(this._noIssueFoundView);

            SelectedView = this._noIssueFoundView;
        }

        public void ShowConnection(object sender, EventArgs e)
        {
            this.StopLoading();
            this.EnableCommand(true, this._service, Guids.COMMAND_HOME_ID);
            this.EnableCommand(true, this._service, Guids.COMMAND_FILTERS_ID);
            this.EnableCommand(true, this._service, Guids.COMMAND_ADVANCED_SEARCH_ID);
            this.EnableCommand(true, this._service, Guids.COMMAND_REFRESH_ID);
            this.EnableCommand(true, this._service, Guids.COMMAND_CONNECTION_ID);

            this._connectionView = new ConnectionView(this, this._userService);
            this._historyNavigator.AddView(this._connectionView);

            SelectedView = this._connectionView;
        }

        public void GoBack(object sender, EventArgs e)
        {
            if (this._historyNavigator.CanGoBack())
            {
                this.SelectedView = this._historyNavigator.GetBackView();
                var selView = ((UserControl)this._selectedView).DataContext;

                if (selView is IReinitializable)
                {
                    (selView as IReinitializable).Reinitialize();
                }
            }
        }

        public void GoForward(object sender, EventArgs e)
        {
            if (this._historyNavigator.CanGoForward())
            {
                this.SelectedView = this._historyNavigator.GetForwardView();
                var selView = ((UserControl)this._selectedView).DataContext;

                if (selView is IReinitializable)
                {
                    (selView as IReinitializable).Reinitialize();
                }
            }
        }

        private void InitializeCommands(OleMenuCommandService service)
        {
            if (service != null)
            {
                CommandID toolbarMenuCommandHomeID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_HOME_ID);
                CommandID toolbarMenuCommandBackID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_BACK_ID);
                CommandID toolbarMenuCommandForwardID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_FORWARD_ID);
                CommandID toolbarMenuCommandConnectionID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_CONNECTION_ID);
                toolbarMenuCommandRefreshID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_REFRESH_ID);
                CommandID toolbarMenuCommandFiltersID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_FILTERS_ID);
                CommandID toolbarMenuCommandAdvancedSearchID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_ADVANCED_SEARCH_ID);

                MenuCommand onToolbarMenuCommandHomeClick = new MenuCommand(ShowProjects, toolbarMenuCommandHomeID);
                MenuCommand onToolbarMenuCommandBackClick = new MenuCommand(GoBack, toolbarMenuCommandBackID);
                MenuCommand onToolbarMenuCommandForwardClick = new MenuCommand(GoForward, toolbarMenuCommandForwardID);
                MenuCommand onToolbarMenuCommandConnectionClick = new MenuCommand(ShowConnection, toolbarMenuCommandConnectionID);
                MenuCommand onToolbarMenuCommandRefreshClick = new MenuCommand(null, toolbarMenuCommandRefreshID);
                MenuCommand onToolbarMenuCommandFiltersClick = new MenuCommand(ShowFilters, toolbarMenuCommandFiltersID);
                MenuCommand onToolbarMenuCommandAdvancedSearClick = new MenuCommand(ShowAdvancedSearch, toolbarMenuCommandAdvancedSearchID);

                service.AddCommand(onToolbarMenuCommandHomeClick);
                service.AddCommand(onToolbarMenuCommandBackClick);
                service.AddCommand(onToolbarMenuCommandForwardClick);
                service.AddCommand(onToolbarMenuCommandConnectionClick);
                service.AddCommand(onToolbarMenuCommandRefreshClick);
                service.AddCommand(onToolbarMenuCommandFiltersClick);
                service.AddCommand(onToolbarMenuCommandAdvancedSearClick);
            }
        }

        public void SetRefreshCommand(EventHandler command)
        {
            this._service.RemoveCommand(this._service.FindCommand(toolbarMenuCommandRefreshID));

            MenuCommand onToolbarMenuCommandRefreshClick = new MenuCommand(command, toolbarMenuCommandRefreshID);

            this._service.AddCommand(onToolbarMenuCommandRefreshClick);
        }

        private void EnableCommand(bool enable, OleMenuCommandService service, int commandGuid)
        {
            if (service != null)
            {
                CommandID toolbarMenuCommandID = new CommandID(Guids.guidJiraToolbarMenu, commandGuid);
                MenuCommand onToolbarMenuCommandClick = service.FindCommand(toolbarMenuCommandID);

                onToolbarMenuCommandClick.Enabled = enable;
            }
        }

        public void SetPanelTitles(string title, string subtitle)
        {
            this.Title = title;
            this.Subtitle = subtitle;
        }

        public void StartLoading()
        {
            this.IsLoading = true;
        }

        public void StopLoading()
        {
            this.IsLoading = false;
        }

        public void SetErrorMessage(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public object SelectedView
        {
            get { return _selectedView; }
            set
            {
                this._selectedView = value;
                this.SetErrorMessage(null);

                var selView = ((UserControl)this._selectedView).DataContext as ITitleable;
                selView.SetPanelTitles();

                if (this._historyNavigator.CanGoBack())
                {
                    this.EnableCommand(true, _service, Guids.COMMAND_BACK_ID);
                }
                else
                {
                    this.EnableCommand(false, _service, Guids.COMMAND_BACK_ID);
                }

                if (this._historyNavigator.CanGoForward())
                {
                    this.EnableCommand(true, _service, Guids.COMMAND_FORWARD_ID);
                }
                else
                {
                    this.EnableCommand(false, _service, Guids.COMMAND_FORWARD_ID);
                }

                OnPropertyChanged("SelectedView");
            }
        }

        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Subtitle
        {
            get { return this._subtitle; }
            set
            {
                this._subtitle = value;
                OnPropertyChanged("Subtitle");
            }
        }

        public bool IsLoading
        {
            get { return this._isLoading; }
            set
            {
                this._isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public string ErrorMessage
        {
            get { return this._errorMessage; }
            set
            {
                this._errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public HistoryNavigator HistoryNavigator
        {
            get { return this._historyNavigator; }
        }
    }
}
