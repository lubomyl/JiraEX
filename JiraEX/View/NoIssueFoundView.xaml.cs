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
    /// Interaction logic for NoIssueFoundView.xaml
    /// </summary>
    public partial class NoIssueFoundView : UserControl
    {
        private NoIssueFoundViewModel _viewModel;

        public NoIssueFoundView(IJiraToolWindowNavigatorViewModel parent, string issueKey)
        {
            InitializeComponent();

            this._viewModel = new NoIssueFoundViewModel(parent, issueKey);
            this.DataContext = this._viewModel;
        }
    }
}
