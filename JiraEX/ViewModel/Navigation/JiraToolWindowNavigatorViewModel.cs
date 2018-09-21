using ConfluenceEX.Helper;
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

        private OleMenuCommandService _service;

        private string _title;
        private string _subtitle;

        public JiraToolWindowNavigatorViewModel(JiraToolWindow parent)
        {
            this._parent = parent;

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
            try
            {
                string accessToken = UserSettingsHelper.ReadFromUserSettings("JiraAccessToken");
                string accessTokenSecret = UserSettingsHelper.ReadFromUserSettings("JiraAccessTokenSecret");
                string baseUrl = UserSettingsHelper.ReadFromUserSettings("JiraBaseUrl");

                if (accessToken != null && accessTokenSecret != null && baseUrl != null)
                {
                    this._oAuthService.ReinitializeOAuthSessionAccessToken(accessToken, accessTokenSecret, baseUrl);

                    this.ShowAfterSignIn();
                }
            }
            catch (Exception ex)
            {
                this.ShowBeforeSignIn();
            }
        }

        public void ShowBeforeSignIn()
        {
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
            if (this._afterSignInView == null)
            {
                this._afterSignInView = new AfterSignInView(this, this._userService, this._oAuthService);

                SelectedView = this._afterSignInView;
            }
            else
            {
                SelectedView = _afterSignInView;
            }
        }

        public void ShowOAuthVerificationConfirmation(object sender, EventArgs e, IToken requestToken)
        {
            this._oAuthVerifierConfirmationView = new OAuthVerifierConfirmationView(this, requestToken, this._oAuthService);

            SelectedView = this._oAuthVerifierConfirmationView;
        }

        public void ShowProjects(object sender, EventArgs e)
        {
            if (this._projectListView == null)
            {
                this._projectListView = new ProjectListView(this, this._projectService, this._boardService);

                SelectedView = this._projectListView;
            }
            else
            {
                SelectedView = this._projectListView;
            }
        }

        public void ShowIssuesOfProject(BoardProject boardProject)
        {
            this._issueListView = new IssueListView(this, boardProject, this._issueService, this._sprintService);

            SelectedView = this._issueListView;
        }

        public void ShowIssueDetail(Issue issue, BoardProject project)
        {
            this._issueDetailView = new IssueDetailView(this, issue, project,
                this._issueService, this._priorityService, this._transitionService,
                this._attachmentService, this._userService, this._boardService);

            SelectedView = this._issueDetailView;
        }

        public void CreateIssue(BoardProject project)
        {
            this._createIssueView = new CreateIssueView(this, project, this._issueService);

            SelectedView = this._createIssueView;
        }

        //sub-task overload
        public void CreateIssue(Issue parentIssue, BoardProject project)
        {
            this._createIssueView = new CreateIssueView(this, parentIssue, project, this._issueService);

            SelectedView = this._createIssueView;
        }

        public void ShowFilters(object sender, EventArgs e)
        {
            if (this._filtersListView == null)
            {
                this._filtersListView = new FiltersListView(this);

                SelectedView = this._filtersListView;
            }
            else
            {
                SelectedView = this._filtersListView;
            }
        }

        private void InitializeCommands(OleMenuCommandService service)
        {
            if (service != null)
            {
                CommandID toolbarMenuCommandHomeID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_HOME_ID);
                CommandID toolbarMenuCommandConnectionID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_CONNECTION_ID);
                CommandID toolbarMenuCommandFiltersID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_FILTERS_ID);

                MenuCommand onToolbarMenuCommandHomeClick = new MenuCommand(ShowProjects, toolbarMenuCommandHomeID);
                MenuCommand onToolbarMenuCommandConnectionClick = new MenuCommand(ShowConnection, toolbarMenuCommandConnectionID);
                MenuCommand onToolbarMenuCommandFiltersClick = new MenuCommand(ShowFilters, toolbarMenuCommandFiltersID);

                service.AddCommand(onToolbarMenuCommandHomeClick);
                service.AddCommand(onToolbarMenuCommandConnectionClick);
                service.AddCommand(onToolbarMenuCommandFiltersClick);
            }
        }

        public void SetPanelTitles(string title, string subtitle)
        {
            this.Title = title;
            this.Subtitle = subtitle;
        }

        public object SelectedView
        {
            get { return _selectedView; }
            set
            {
                this._selectedView = value;
                var selView = ((UserControl)this._selectedView).DataContext as ITitleable;
                selView.SetPanelTitles();

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
    }
}
