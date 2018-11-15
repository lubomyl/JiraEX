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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace JiraEX.ViewModel
{
    public class AuthenticationVerificationViewModel : ViewModelBase, ITitleable
    {

        private const int REQUEST_TOKEN_EXPIRATION_TIME_SECONDS = 600;

        private IJiraToolWindowNavigatorViewModel _parent;

        private IOAuthService _oAuthService;

        private string _oAuthVerificationCode;
        private IToken _requestToken;

        DispatcherTimer _timer;
        TimeSpan _time;

        private string _requestTokenExpirationTime;

        private WritableSettingsStore _userSettingsStore;

        public DelegateCommand SignInCommand { get; private set; }

        public AuthenticationVerificationViewModel(IJiraToolWindowNavigatorViewModel parent, IToken requestToken, IOAuthService oAuthService)
        {
            this._parent = parent;

            this._oAuthService = oAuthService;

            this._requestToken = requestToken;

            this.StartRequestTokenExpireTimeCountdown();

            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            this._userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            this.SignInCommand = new DelegateCommand(SignIn);

            SetPanelTitles();
        }

        private async void SignIn(object parameter)
        {
            try
            {
                IToken accessToken = await this._oAuthService.ExchangeRequestTokenForAccessToken(this._requestToken, OAuthVerificationCode);

                UserSettingsHelper.WriteToUserSettings("JiraAccessToken", accessToken.Token);
                UserSettingsHelper.WriteToUserSettings("JiraAccessTokenSecret", accessToken.TokenSecret);

                WriteToUserSettingsIssueAttributes();

                this._parent.ShowConnection();
                this._timer.Stop();
            }
            catch (OAuthException ex)
            {
                this._parent.SetErrorMessage(ex.Message);
            }
        }

        private void WriteToUserSettingsIssueAttributes()
        {
            UserSettingsHelper.WriteToUserSettings("TypeAttribute", true);
            UserSettingsHelper.WriteToUserSettings("StatusAttribute", true);
            UserSettingsHelper.WriteToUserSettings("CreatedAttribute", false);
            UserSettingsHelper.WriteToUserSettings("UpdatedAttribute", false);
            UserSettingsHelper.WriteToUserSettings("AssigneeAttribute", false);
            UserSettingsHelper.WriteToUserSettings("SummaryAttribute", true);
            UserSettingsHelper.WriteToUserSettings("PriorityAttribute", true);
        }

        public string OAuthVerificationCode
        {
            get
            {
                return this._oAuthVerificationCode;
            }
            set
            {
                this._oAuthVerificationCode = value;
                OnPropertyChanged("OAuthVerificationCode");
            }
        }

        private void InitializeRequestTokenExpireTimeCountdown(TimeSpan time)
        {
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.RequestTokenExpirationTime = time.ToString();

                if (time == TimeSpan.Zero)
                {
                    _timer.Stop();
                    this._parent.ShowAuthentication();
                }

                time = time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
        }

        private void StartRequestTokenExpireTimeCountdown()
        {
            _time = TimeSpan.FromSeconds(REQUEST_TOKEN_EXPIRATION_TIME_SECONDS);

            this.InitializeRequestTokenExpireTimeCountdown(_time);

            _timer.Start();
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Authentication verification");
        }

        public string RequestTokenExpirationTime
        {
            get
            {
                return this._requestTokenExpirationTime;
            }
            set
            {
                this._requestTokenExpirationTime = value;
                OnPropertyChanged("RequestTokenExpirationTime");
            }
        }
    }
}

