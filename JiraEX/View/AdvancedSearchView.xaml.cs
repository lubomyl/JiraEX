using JiraEX.ViewModel;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
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

    public partial class AdvancedSearchView : UserControl
    {

        private AdvancedSearchViewModel _viewModel;

        public AdvancedSearchView(IJiraToolWindowNavigatorViewModel parent, IPriorityService priorityService, 
            IIssueService issueService, IProjectService projectService)
        {
            InitializeComponent();

            this._viewModel = new AdvancedSearchViewModel(parent, priorityService, issueService, projectService);
            this.DataContext = this._viewModel;
        }

        void IssueSelected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            Issue issue = listBox.SelectedItem as Issue;

            this._viewModel.OnItemSelected(issue);
        }
    }
}
