using JiraEX.ViewModel;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
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
    /// Interaction logic for WorklogView.xaml
    /// </summary>
    public partial class WorklogView : UserControl
    {

        private WorklogViewModel _viewModel;

        public WorklogView(Issue issue)
        {
            InitializeComponent();

            this._viewModel = new WorklogViewModel(issue);
            this.DataContext = this._viewModel;
        }
    }
}
