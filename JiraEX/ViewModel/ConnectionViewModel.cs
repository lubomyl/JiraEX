using AtlassianConnector.Model.Exceptions;
using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class ConnectionViewModel : ViewModelBase, ITitleable
    {

        private IJiraToolWindowNavigatorViewModel _parent;

        private User _authenticatedUser;

        private IUserService _userService;
        private IBasicAuthenticationService _basicAuthenticationService;

        private WritableSettingsStore _userSettingsStore;

        public DelegateCommand SignOutCommand { get; private set; }
        public DelegateCommand IssueReportGitHubCommand { get; private set; }

        public ConnectionViewModel(IJiraToolWindowNavigatorViewModel parent, IUserService userService, IBasicAuthenticationService basicAuthenticationService)
        {
            this._parent = parent;
            
            this._userService = userService;
            this._basicAuthenticationService = basicAuthenticationService;

            this.GetAuthenticatedUserAsync();

            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            this._userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            this.SignOutCommand = new DelegateCommand(SignOut);
            this.IssueReportGitHubCommand = new DelegateCommand(IssueReportGitHub);

            SetPanelTitles();
        }

        private async void GetAuthenticatedUserAsync()
        {
            try
            {
                Task<User> authenticatedUserTask = this._userService.GetAuthenticatedUserAsync();

                this.AuthenticatedUser = await authenticatedUserTask as User;
            }
            catch (OAuthException ex)
            {
                this._parent.ShowAuthentication();
            }
            catch (JiraException ex2)
            {
                this._parent.ShowAuthentication();

                ShowErrorMessages(ex2, this._parent);
            }

            this._parent.StopLoading();
        }

        private void SignOut(object parameter)
        {
            UserSettingsHelper.DeletePropertyFromUserSettings("JiraAccessToken");
            UserSettingsHelper.DeletePropertyFromUserSettings("JiraAccessTokenSecret");
            UserSettingsHelper.DeletePropertyFromUserSettings("JiraBaseUrl");

            this._basicAuthenticationService.DeleteAuthenticationCredentials();

            this._parent.DisposeConnectionView();

            this._parent.ShowAuthentication();
        }

        private void IssueReportGitHub(object parameter)
        {
            string issueReportURL = "https://github.com/lubomyl/JiraEX";

            System.Diagnostics.Process.Start(issueReportURL);
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", UserSettingsHelper.ReadStringFromUserSettings("JiraBaseUrl"));
        }

        public User AuthenticatedUser
        {
            get
            {
                return this._authenticatedUser;
            }
            set
            {
                this._authenticatedUser = value;
                OnPropertyChanged("AuthenticatedUser");
            }
        }

        public string JiraURL
        {
            get { return UserSettingsHelper.ReadStringFromUserSettings("JiraBaseUrl"); }
        }

    }
}
