using AtlassianConnector.Model.Exceptions;
using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.Common;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace JiraEX.ViewModel
{
    public class AuthenticationViewModel : ViewModelBase, ITitleable
    {

        private IOAuthService _oAuthService;
        private IBasicAuthenticationService _basicService;
        private IUserService _userService;

        private IJiraToolWindowNavigatorViewModel _parent;

        private string _baseUrl;
        private string _baseUrlBasic;
        private string _username;
        private string _password;

        private WritableSettingsStore _userSettingsStore;

        public DelegateCommand SignInOAuthCommand { get; private set; }
        public DelegateCommand SignInBasicCommand { get; private set; }
        public DelegateCommand HowToSetupOAuthCommand { get; private set; }

        public AuthenticationViewModel(IJiraToolWindowNavigatorViewModel parent, 
            IOAuthService oAuthService, 
            IBasicAuthenticationService basicService,
            IUserService userService)
        {
            this._parent = parent;

            this._baseUrl = UserSettingsHelper.ReadStringFromUserSettings("JiraBaseUrl");
            this._baseUrlBasic = UserSettingsHelper.ReadStringFromUserSettings("JiraBaseUrl");
            this._username = UserSettingsHelper.ReadStringFromUserSettings("JiraUsername");

            this._oAuthService = oAuthService;
            this._basicService = basicService;
            this._userService = userService;

            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            this._userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            this.SignInOAuthCommand = new DelegateCommand(SignInOAuth);
            this.SignInBasicCommand = new DelegateCommand(SignInBasic);
            this.HowToSetupOAuthCommand = new DelegateCommand(HowToSetupOAuth);

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

                this._parent.InitializeServicesWithAuthenticationType(AuthenticationType.OAuth);

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

        private void SignInBasic(object parameter)
        {
            try
            {
                var passwordContainer = parameter as IHavePassword;
                if (passwordContainer != null)
                {
                    var secureString = passwordContainer.Password;
                    this._password = ConvertToUnsecureString(secureString);
                }

                this._parent.StartLoading();

                this._baseUrlBasic = this.ProcessBaseUrlInput(this.BaseUrlBasic);

                UserSettingsHelper.WriteToUserSettings("JiraBaseUrl", this.BaseUrlBasic);
                UserSettingsHelper.WriteToUserSettings("JiraUsername", this.Username);

                this._basicService.InitializeBasicAuthenticationAuthenticator(this.BaseUrlBasic, this.Username, this._password);

                this._parent.InitializeServicesWithAuthenticationType(AuthenticationType.Basic);

                this._parent.AreUserCredentialsCorrect();
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
        }

        private void HowToSetupOAuth(object sender)
        {
            string howToURL = "https://github.com/lubomyl/JiraEX/wiki/How-to-setup-OAuth-(Jira-administrator)";

            System.Diagnostics.Process.Start(howToURL);
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
    
            if(ret == null)
            {
                ret = "";
            }

            return ret;
        }

        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Authentication");
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

        public string BaseUrlBasic
        {
            get
            {
                return this._baseUrlBasic;
            }
            set
            {
                this._baseUrlBasic = value;
                OnPropertyChanged("BaseUrlBasic");
            }
        }

        public string Username
        {
            get
            {
                return this._username;
            }
            set
            {
                this._username = value;
                OnPropertyChanged(Username);
            }
        }

        #endregion
    }
}