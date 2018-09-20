using DevDefined.OAuth.Framework;
using JiraEX.ViewModel;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JiraEX.View
{
    /// <summary>
    /// Interaction logic for OAuthVerifierConfirmationView.xaml
    /// </summary>
    public partial class OAuthVerifierConfirmationView : UserControl
    {

        private OAuthVerifierConfirmationViewModel _viewModel;

        public OAuthVerifierConfirmationView(JiraToolWindowNavigatorViewModel parent, IToken requestToken, IOAuthService oAuthService)
        {
            InitializeComponent();

            this._viewModel = new OAuthVerifierConfirmationViewModel(parent, requestToken, oAuthService);
            this.DataContext = this._viewModel;
        }
    }
}
