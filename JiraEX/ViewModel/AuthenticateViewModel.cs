using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace JiraEX.ViewModel
{
    public class AuthenticateViewModel : ViewModelBase, ITitleable
    {

        private IOAuthService _oAuthService;

        private IJiraToolWindowNavigatorViewModel _parent;

        private string _baseUrl;

        private WritableSettingsStore _userSettingsStore;

        public DelegateCommand SignInOAuthCommand { get; private set; }

        public AuthenticateViewModel(IJiraToolWindowNavigatorViewModel parent, IOAuthService oAuthService)
        {
            this._parent = parent;

            this._oAuthService = oAuthService;

            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            this._userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            this.SignInOAuthCommand = new DelegateCommand(SignInOAuth);

            SetPanelTitles();
        }

        private async void SignInOAuth(object parameter)
        {
            IToken requestToken;
            string authorizationUrl;

            try
            {
                this._parent.StartLoading();

                this._baseUrl = this.ProcessBaseUrlInput(this.BaseUrl);

                this._oAuthService.InitializeOAuthSession(this.BaseUrl);

                UserSettingsHelper.WriteToUserSettings("JiraBaseUrl", this.BaseUrl);

                requestToken = await this._oAuthService.GetRequestToken();
                authorizationUrl = await this._oAuthService.GetUserAuthorizationUrlForToken(requestToken);

                System.Diagnostics.Process.Start(authorizationUrl);
                this._parent.ShowAuthenticationVerification(null, null, requestToken);
            }
            catch (OAuthException ex)
            {
                this._parent.SetErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                this._parent.SetErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                this._parent.SetErrorMessage(ex.Message);
            }

            this._parent.StopLoading();
        }

        private string ProcessBaseUrlInput(string baseUrl)
        {
            string ret = baseUrl;
            string https = "https://";
            string http = "http://";

            if (baseUrl.Length > 7)
            {
                if (!baseUrl.Substring(0, 8).Equals(https) && !baseUrl.Substring(0, 7).Equals(http))
                {
                    ret = https + ret;
                }
            }

            return ret;
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "OAuth authentication");
        }

        #region BeforeSignInViewModel Members   

        public string BaseUrl
        {
            get
            {
                return this._baseUrl;
            }
            set
            {
                this._baseUrl = value;
                OnPropertyChanged("BaseUrl");
            }
        }

        #endregion
    }
}