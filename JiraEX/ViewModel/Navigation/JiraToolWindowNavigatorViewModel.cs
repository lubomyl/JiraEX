using ConfluenceEX.Helper;
using JiraEX.View;
using JiraRESTClient.Service;
using System;
using System.Collections.Generic;
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

        public JiraToolWindowNavigatorViewModel(JiraToolWindow parent)
        {
            this._parent = parent;
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

                    //this.ShowAfterSignIn();
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
