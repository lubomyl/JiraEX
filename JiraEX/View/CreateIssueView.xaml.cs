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
    /// Interaction logic for CreateIssueView.xaml
    /// </summary>
    public partial class CreateIssueView : UserControl
    {

        private CreateIssueViewModel _viewModel;

        public CreateIssueView(JiraToolWindowNavigatorViewModel parent, BoardProject project)
        {
            InitializeComponent();

            this._viewModel = new CreateIssueViewModel(parent, project);
            this.DataContext = this._viewModel;
        }

        //sub-task overload
        public CreateIssueView(JiraToolWindowNavigatorViewModel parent, Issue parentIssue, BoardProject project)
        {
            InitializeComponent();

            this._viewModel = new CreateIssueViewModel(parent, parentIssue, project);
            this.DataContext = this._viewModel;
        }
    }
}
