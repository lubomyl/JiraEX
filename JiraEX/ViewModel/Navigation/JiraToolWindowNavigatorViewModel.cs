using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.Main;
using JiraEX.View;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel.Navigation
{
    public class JiraToolWindowNavigatorViewModel : ViewModelBase
    {

        private object _selectedView;
        private JiraToolWindow _parent;

        private IOAuthService _oAuthService;

        private BeforeSignInView _beforeSignInView;
        private OAuthVerifierConfirmationView _oAuthVerifierConfirmationView;
        private AfterSignInView _afterSignInView;
        private ProjectListView _projectListView;

        private OleMenuCommandService _service;

        public JiraToolWindowNavigatorViewModel(JiraToolWindow parent)
        {
            this._parent = parent;

            this._service = JiraPackage.Mcs;

            this._oAuthService = new OAuthService();

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
            _parent.Caption = "Jira - Sign-in";

            if (this._beforeSignInView == null)
            {
                this._beforeSignInView = new BeforeSignInView(this);

                SelectedView = this._beforeSignInView;
            }
            else
            {
                SelectedView = _beforeSignInView;
            }
        }

        public void ShowAfterSignIn()
        {
            _parent.Caption = "Jira - Signed-in";

            if (this._afterSignInView == null)
            {
                this._afterSignInView = new AfterSignInView(this);

                SelectedView = this._afterSignInView;
            }
            else
            {
                SelectedView = _afterSignInView;
            }
        }

        public void ShowOAuthVerificationConfirmation(object sender, EventArgs e, IToken requestToken)
        {
            _parent.Caption = "Confirm OAuth Verification Code";

            this._oAuthVerifierConfirmationView = new OAuthVerifierConfirmationView(this, requestToken);

            SelectedView = this._oAuthVerifierConfirmationView;
        }

        public void ShowProjects(object sender, EventArgs e)
        {
            _parent.Caption = "Jira Projects";

            if (this._projectListView == null)
            {
                this._projectListView = new ProjectListView(this);

                SelectedView = this._projectListView;
            }
            else
            {
                SelectedView = this._projectListView;
            }
        }

        private void InitializeCommands(OleMenuCommandService service)
        {
            if (service != null)
            {
                CommandID toolbarMenuCommandHomeID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_HOME_ID);
                CommandID toolbarMenuCommandConnectionID = new CommandID(Guids.guidJiraToolbarMenu, Guids.COMMAND_CONNECTION_ID);

                MenuCommand onToolbarMenuCommandHomeClick = new MenuCommand(ShowProjects, toolbarMenuCommandHomeID);
                MenuCommand onToolbarMenuCommandConnectionClick = new MenuCommand(ShowConnection, toolbarMenuCommandConnectionID);

                service.AddCommand(onToolbarMenuCommandHomeClick);
                service.AddCommand(onToolbarMenuCommandConnectionClick);
            }
        }

        public object SelectedView
        {
            get { return _selectedView; }
            set
            {
                this._selectedView = value;
                OnPropertyChanged("SelectedView");
            }
        }
    }
}
