using JiraEX.ViewModel;
using JiraEX.ViewModel.Navigation;
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
    /// Interaction logic for AfterSignInView.xaml
    /// </summary>
    public partial class AfterSignInView : UserControl
    {

        private AfterSignInViewModel _viewModel;

        public AfterSignInView(JiraToolWindowNavigatorViewModel parent)
        {
            InitializeComponent();

            this._viewModel = new AfterSignInViewModel(parent);
            this.DataContext = this._viewModel;
        }
    }
}
