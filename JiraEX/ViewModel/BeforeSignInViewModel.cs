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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace JiraEX.ViewModel
{
    public class BeforeSignInViewModel : ViewModelBase
    {

        private IOAuthService _oAuthService;

        private JiraToolWindowNavigatorViewModel _parent;

        private string _errorMessage;
        private string _baseUrl;

        private WritableSettingsStore _userSettingsStore;

        //public DelegateCommand SignInOAuthCommand { get; private set; }

        public BeforeSignInViewModel(JiraToolWindowNavigatorViewModel parent)
        {
            this._parent = parent;

            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            this._userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            //this.SignInOAuthCommand = new DelegateCommand(SignInOAuth);
        }

        /*private async void SignInOAuth(object parameter)
        {
            this._oAuthService = new OAuthService();

            IToken requestToken;
            string authorizationUrl;

            try
            {
                this._baseUrl = this.ProcessBaseUrlInput(this.BaseUrl);

                this._oAuthService.InitializeOAuthSession(this.BaseUrl);

                UserSettingsHelper.WriteToUserSettings("ConfluenceBaseUrl", this.BaseUrl);

                requestToken = await this._oAuthService.GetRequestToken();
                authorizationUrl = await this._oAuthService.GetUserAuthorizationUrlForToken(requestToken);

                System.Diagnostics.Process.Start(authorizationUrl);
                this._parent.ShowOAuthVerificationConfirmation(null, null, requestToken);
            }
            catch (OAuthException ex)
            {
                this.ErrorMessage = ex.Message;
            }
            catch (SecurityException ex)
            {
                this.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
        }*/

        private string ProcessBaseUrlInput(string baseUrl)
        {
            string ret = baseUrl;
            string https = "https://";

            if (!baseUrl.Substring(0, 8).Equals("https://"))
            {
                ret = https + ret;
            }

            return ret;
        }

        #region BeforeSignInViewModel Members   

        public string ErrorMessage
        {
            get
            {
                return this._errorMessage;
            }
            set
            {
                this._errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

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