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
    /// Interaction logic for BeforeSignInView.xaml
    /// </summary>
    public partial class BeforeSignInView : UserControl
    {

        private BeforeSignInViewModel _viewModel;

        public BeforeSignInView(JiraToolWindowNavigatorViewModel parent, IOAuthService oAuthService)
        {
            InitializeComponent();

            this._viewModel = new BeforeSignInViewModel(parent, oAuthService);
            this.DataContext = this._viewModel;
        }
    }
}
