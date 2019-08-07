using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Service;
using JiraEX.ViewModel;
using System.Threading.Tasks;
using JiraRESTClient.Model;

namespace JiraEX.UnitTests.ViewModel
{
    [TestClass]
    public class FilterListViewModelUnitTests
    {
        private const int NUMBER_OF_FILTERS = 10;

        Mock<IJiraToolWindowNavigatorViewModel> _mockJiraToolWindowNavigatorViewModel;
        Mock<IIssueService> _mockIssueService;

        Mock<List<Filter>> _mockFilterList;
        Mock<Filter> _mockFilter;

        FilterListViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            this._mockJiraToolWindowNavigatorViewModel = new Mock<IJiraToolWindowNavigatorViewModel>();

            this._mockFilterList = new Mock<List<Filter>>();
            this._mockFilter = new Mock<Filter>();

            this._mockIssueService = new Mock<IIssueService>();

            this._mockIssueService.Setup(mock => mock.GetAllFiltersAsync()).Returns(Task.FromResult(this._mockFilterList.Object));

            Add_Filters();

            this._viewModel = new FilterListViewModel(_mockJiraToolWindowNavigatorViewModel.Object,
                this._mockIssueService.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void NoFilters_State_Changed_If_ProjectList_Is_Empty()
        {
            this._mockFilterList.Object.Clear();

            this._viewModel = new FilterListViewModel(_mockJiraToolWindowNavigatorViewModel.Object,
                this._mockIssueService.Object);

            Assert.AreEqual(this._viewModel.FilterList.Count, 0);
            Assert.IsTrue(this._viewModel.NoFilters);
        }

        [TestMethod]
        public void NoFilters_State_False_If_ProjectList_Is_Not_Empty()
        {
            Assert.AreEqual(this._viewModel.FilterList.Count, NUMBER_OF_FILTERS);
            Assert.IsFalse(this._viewModel.NoFilters);
        }

        private void Add_Filters()
        {
            this._mockFilterList.Object.Clear();

            for (int i = 0; i < NUMBER_OF_FILTERS; i++)
            {
                this._mockFilterList.Object.Add(_mockFilter.Object);
            }
        }
    }
}
