﻿using ConfluenceEX.Command;
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
    public class AfterSignInViewModel : ViewModelBase
    {

        private JiraToolWindowNavigatorViewModel _parent;

        private User _authenticatedUser;

        private IUserService _userService;

        private IOAuthService _oauthService;

        private WritableSettingsStore _userSettingsStore;

        public DelegateCommand SignOutCommand { get; private set; }

        public AfterSignInViewModel(JiraToolWindowNavigatorViewModel parent)
        {
            this._parent = parent;

            this._oauthService = new OAuthService();
            this._userService = new UserService();
            this.GetAuthenticatedUserAsync();

            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            this._userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            this.SignOutCommand = new DelegateCommand(SignOut);
        }

        private async void GetAuthenticatedUserAsync()
        {
            try
            {
                System.Threading.Tasks.Task<User> authenticatedUserTask = this._userService.GetAuthenticatedUserAsync();

                this.AuthenticatedUser = await authenticatedUserTask as User;
            }
            catch (OAuthException ex)
            {
                this._parent.ShowBeforeSignIn();
            }
        }

        private void SignOut(object parameter)
        {
            UserSettingsHelper.DeletePropertyFromUserSettings("JiraAccessToken");
            UserSettingsHelper.DeletePropertyFromUserSettings("JiraAccessTokenSecret");
            UserSettingsHelper.DeletePropertyFromUserSettings("JiraBaseUrl");

            this._parent.ShowBeforeSignIn();
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

    }
}
