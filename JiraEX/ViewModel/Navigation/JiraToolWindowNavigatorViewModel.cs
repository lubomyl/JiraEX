using ConfluenceEX.Helper;
using ConfluenceEX.ViewModel.Navigation;
using DevDefined.OAuth.Framework;
using JiraEX.Main;
using JiraEX.View;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JiraEX.ViewModel.Navigation
{
    public class JiraToolWindowNavigatorViewModel : ViewModelBase, IJiraToolWindowNavigatorViewModel
    {

        private object _selectedView;
        private JiraToolWindow _parent;

        private IUserService _userService;
        private IOAuthService _oAuthService;
        private IIssueService _issueService;
        private ITransitionService _transitionService;
        private IPriorityService _priorityService;
        private IAttachmentService _attachmentService;
        private IBoardService _boardService;
        private ISprintService _sprintService;
        private IProjectService _projectService;

        private BeforeSignInView _beforeSignInView;
        private OAuthVerifierConfirmationView _oAuthVerifierConfirmationView;
        private AfterSignInView _afterSignInView;
        private ProjectListView _projectListView;
        private IssueListView _issueListView;
        private IssueDetailView _issueDetailView;
        private CreateIssueView _createIssueView;
        private FiltersListView _filtersListView;
        private AdvancedSearchView _advancedSearchView;

        private HistoryNavigator _historyNavigator;

        private OleMenuCommandService _service;

        private string _title;
        private string _subtitle;

        private bool _isLoading;

        public JiraToolWindowNavigatorViewModel(JiraToolWindow parent)
        {
            this._parent = parent;

            this._historyNavigator = new HistoryNavigator();
            
            this._service = JiraPackage.Mcs;

            this._userService = new UserService();
            this._oAuthService = new OAuthService();
            this._issueService = new IssueService();
            this._transitionService = new TransitionService();
            this._priorityService = new PriorityService();
            this._attachmentService = new AttachmentService();
            this._boardService = new BoardService();
            this._sprintService = new SprintService();
            this._projectService = new ProjectService();

            InitializeCommands(this._service);
        }

        public void ShowConnection(object sender, EventArgs e)
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

                    this.ShowAfterSignIn();
                } else
                {
                    this.ShowBeforeSignIn();
                }
            }
            catch (Exception ex)
            {
                this.ShowBeforeSignIn();
            }
        }

        public void ShowBeforeSignIn()
        {
            this.StopLoading();

            if (this._beforeSignInView == null)
            {
                this._beforeSignInView = new BeforeSignInView(this, this._oAuthService);

                SelectedView = this._beforeSignInView;
            }
            else
            {
                SelectedView = _beforeSignInView;
            }
        }

        public void ShowAfterSignIn()
        {
            this.StopLoading();

            if (this._afterSignInView == null)
            {
                this._afterSignInView = new AfterSignInView(this, this._userService, this._oAuthService);
                this._historyNavigator.AddView(this._afterSignInView);

                SelectedView = this._afterSignInView;
            }
            else
            {
                this._historyNavigator.AddView(this._afterSignInView);

                SelectedView = _afterSignInView;
            }
        }

        public void ShowOAuthVerificationConfirmation(object sender, EventArgs e, IToken requestToken)
        {
            this.StopLoading();

            this._oAuthVerifierConfirmationView = new OAuthVerifierConfirmationView(this, requestToken, this._oAuthService);

            SelectedView = this._oAuthVerifierConfirmationView;
        }

        public void ShowProjects(object sender, EventArgs e)
        {
            this.StopLoading();

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

            this._issueListView = new IssueListView(this, this._issueService, project);
            this._historyNavigator.AddView(this._issueListView);

            SelectedView = this._issueListView;
        }

        public void ShowIssuesOfFilter(Filter filter)
        {
            this.StopLoading();

            this._issueListView = new IssueListView(this, this._issueService, this._sprintService, filter.Jql);
            this._historyNavigator.AddView(this._issueListView);

            SelectedView = this._issueListView;
        }

        public void ShowIssueDetail(Issue issue, Project project)
        {
            this.StopLoading();

            this._issueDetailView = new IssueDetailView(this, issue, project, 
                this._issueService, this._priorityService, this._transitionService,
                this._attachmentService, this._userService, this._boardService, this._projectService);
            this._historyNavigator.AddView(this._issueDetailView);

            SelectedView = this._issueDetailView;
        }

        public void ShowCreateIssue(Project project)
        {
            this.StopLoading();

            this._createIssueView = new CreateIssueView(this, project, this._issueService);
            this._historyNavigator.AddView(this._createIssueView);

            SelectedView = this._createIssueView;
        }

        //sub-task overload
        public void ShowCreateIssue(Issue parentIssue, Project project)
        {
            this.StopLoading();

            this._createIssueView = new CreateIssueView(this, parentIssue, project, this._issueService);
            this._historyNavigator.AddView(this._createIssueView);

            SelectedView = this._createIssueView;
        }

        public void ShowFilters(object sender, EventArgs e)
        {
            this.StopLoading();

            this._filtersListView = new FiltersListView(this, this._issueService);
            this._historyNavigator.AddView(this._filtersListView);

            SelectedView = this._filtersListView;
        }

        public void ShowAdvancedSearch(object sender, EventArgs e)
        {
            this.StopLoading();

            this._advancedSearchView = new AdvancedSearchView(this, this._priorityService, this._issueService, this._projectService,
                this._sprintService, this._boardService);
            this._historyNavigator.AddView(this._advancedSearchView);

            SelectedView = this._advancedSearchView;
        }

        private void GoBack(object sender, EventArgs e)
        {
            if (this._historyNavigator.CanGoBack())
            {
                this.SelectedView = this._historyNavigator.GetBackView();
            }
        }

        private void GoForward(object sender, EventArgs e)
        {
            if (this._historyNavigator.CanGoForward())
            {
                this.SelectedView = this._historyNavigator.GetForwardView();
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
                CommandID toolbarMenuCommandFiltersID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_FILTERS_ID);
                CommandID toolbarMenuCommandAdvancedSearchID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_ADVANCED_SEARCH_ID);

                MenuCommand onToolbarMenuCommandHomeClick = new MenuCommand(ShowProjects, toolbarMenuCommandHomeID);
                MenuCommand onToolbarMenuCommandBackClick = new MenuCommand(GoBack, toolbarMenuCommandBackID);
                MenuCommand onToolbarMenuCommandForwardClick = new MenuCommand(GoForward, toolbarMenuCommandForwardID);
                MenuCommand onToolbarMenuCommandConnectionClick = new MenuCommand(ShowConnection, toolbarMenuCommandConnectionID);
                MenuCommand onToolbarMenuCommandFiltersClick = new MenuCommand(ShowFilters, toolbarMenuCommandFiltersID);
                MenuCommand onToolbarMenuCommandAdvancedSearClick = new MenuCommand(ShowAdvancedSearch, toolbarMenuCommandAdvancedSearchID);

                service.AddCommand(onToolbarMenuCommandHomeClick);
                service.AddCommand(onToolbarMenuCommandBackClick);
                service.AddCommand(onToolbarMenuCommandForwardClick);
                service.AddCommand(onToolbarMenuCommandConnectionClick);
                service.AddCommand(onToolbarMenuCommandFiltersClick);
                service.AddCommand(onToolbarMenuCommandAdvancedSearClick);
            }
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

        public object SelectedView
        {
            get { return _selectedView; }
            set
            {
                this._selectedView = value;
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
    }
}
