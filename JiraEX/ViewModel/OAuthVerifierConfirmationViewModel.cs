using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.ViewModel.Navigation;
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
    public class OAuthVerifierConfirmationViewModel : ViewModelBase
    {

        private const int REQUEST_TOKEN_EXPIRATION_TIME_SECONDS = 600;

        private JiraToolWindowNavigatorViewModel _parent;

        private OAuthService _oAuthService;

        private string _oAuthVerificationCode;
        private IToken _requestToken;

        DispatcherTimer _timer;
        TimeSpan _time;

        private string _errorMessage;
        private string _requestTokenExpirationTime;

        private WritableSettingsStore _userSettingsStore;

        public DelegateCommand SignInCommand { get; private set; }

        public OAuthVerifierConfirmationViewModel(JiraToolWindowNavigatorViewModel parent, IToken requestToken)
        {
            this._parent = parent;

            this._requestToken = requestToken;

            this.StartRequestTokenExpireTimeCountdown();

            SettingsManager settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            this._userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            this.SignInCommand = new DelegateCommand(SignIn);
        }

        private async void SignIn(object parameter)
        {
            this._oAuthService = new OAuthService();

            try
            {
                IToken accessToken = await this._oAuthService.ExchangeRequestTokenForAccessToken(this._requestToken, OAuthVerificationCode);

                UserSettingsHelper.WriteToUserSettings("JiraAccessToken", accessToken.Token);
                UserSettingsHelper.WriteToUserSettings("JiraAccessTokenSecret", accessToken.TokenSecret);

                this._parent.ShowAfterSignIn();
                this._timer.Stop();
            }
            catch (OAuthException ex)
            {
                this.ErrorMessage = ex.Message;
            }
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
                    this._parent.ShowBeforeSignIn();
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

