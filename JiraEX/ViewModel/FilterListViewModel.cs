using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class FilterListViewModel : ViewModelBase, ITitleable
    {

        private IJiraToolWindowNavigatorViewModel _parent;

        private IIssueService _issueService;

        private bool _noFilters = false;

        private ObservableCollection<Filter> _filterList;

        public FilterListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService)
        {
            this._parent = parent;

            this._issueService = issueService;

            this._filterList = new ObservableCollection<Filter>();

            GetFiltersAsync();

            SetPanelTitles();
        }

        private async void GetFiltersAsync()
        {
            Task<FilterList> filterTask = this._issueService.GetAllFavouriteFilters();

            var filterList = await filterTask as FilterList;

            if (filterList.Count > 0)
            {
                foreach (Filter f in filterList)
                {
                    this.FilterList.Add(f);
                }
            }
            else
            {
                this.NoFilters = true;
            }
        }

        public void OnItemSelected(object sender)
        {
            Filter filter = sender as Filter;

           // this._parent.Show(filter);
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Favourite filters");
        }

        public ObservableCollection<Filter> FilterList
        {
            get { return this._filterList; }
            set
            {
                this._filterList = value;
                OnPropertyChanged("FilterList");
            }
        }

        public bool NoFilters
        {
            get { return this._noFilters; }
            set
            {
                this._noFilters = value;
                OnPropertyChanged("NoFilters");
            }
        }
    }
}
