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
    /// <summary>
    /// Interaction logic for ProjectsListView.xaml
    /// </summary>
    public partial class ProjectListView : UserControl
    {

        private ProjectListViewModel _viewModel;

        public ProjectListView(JiraToolWindowNavigatorViewModel parent, IProjectService projectService, IBoardService boardService)
        {
            InitializeComponent();

            this._viewModel = new ProjectListViewModel(parent, projectService, boardService);
            this.DataContext = this._viewModel;
        }

        void ProjectSelected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            BoardProject project = listBoxItem.Content as BoardProject;

            this._viewModel.OnItemSelected(project);
        }
    }
}
