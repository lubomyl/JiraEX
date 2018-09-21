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
    /// Interaction logic for FiltersListView.xaml
    /// </summary>
    public partial class FiltersListView : UserControl
    {

        private FilterListViewModel _viewModel;

        public FiltersListView(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService)
        {
            InitializeComponent();

            this._viewModel = new FilterListViewModel(parent, issueService);
            this.DataContext = this._viewModel;
        }

        void FilterSelected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            Filter filter = listBoxItem.Content as Filter;

            this._viewModel.OnItemSelected(filter);
        }
    }
}
