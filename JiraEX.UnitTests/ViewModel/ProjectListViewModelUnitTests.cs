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
    public class ProjectListViewModelUnitTests
    {
        private const int NUMBER_OF_PROJECTS = 10;

        Mock<IJiraToolWindowNavigatorViewModel> _mockJiraToolWindowNavigatorViewModel;
        Mock<IProjectService> _mockProjectService;
        Mock<IBoardService> _mockBoardService;

        Mock<BoardList> _mockBoardList;
        Mock<BoardProject> _mockBoardProject;
        Mock<ProjectCreatableList> _mockProjectCreatableList;

        ProjectListViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            this._mockBoardProject = new Mock<BoardProject>();
            this._mockBoardProject.Object.Location = new Mock<Location>().Object;
            this._mockBoardProject.Object.Location.ProjectId = "0";

            this._mockBoardList = new Mock<BoardList>();
            this._mockBoardList.Object.Values = new List<BoardProject>();

            this._mockProjectCreatableList = new Mock<ProjectCreatableList>();
            this._mockProjectCreatableList.Object.Projects = new List<ProjectCreatable>();

            Mock<ProjectCreatable> _mockProjectCreatable = new Mock<ProjectCreatable>();
            _mockProjectCreatable.Object.Id = "1";

            this._mockProjectCreatableList.Object.Projects.Add(_mockProjectCreatable.Object);

            this._mockJiraToolWindowNavigatorViewModel = new Mock<IJiraToolWindowNavigatorViewModel>();
            this._mockBoardService = new Mock<IBoardService>();
            this._mockProjectService = new Mock<IProjectService>();

            this._mockBoardService.Setup(mock => mock.GetAllBoardsAsync()).Returns(Task.FromResult(this._mockBoardList.Object));
            this._mockProjectService.Setup(mock => mock.GetAllProjectsCreatableIssueTypesAsync()).Returns(Task.FromResult(this._mockProjectCreatableList.Object));

            Add_Ten_BoardProjects();

            this._viewModel = new ProjectListViewModel(_mockJiraToolWindowNavigatorViewModel.Object,
                this._mockProjectService.Object,
                this._mockBoardService.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void BoardProjectList_Is_Initialized()
        {
            Assert.AreEqual(this._viewModel.ProjectList.Count, NUMBER_OF_PROJECTS);
            Assert.IsFalse(this._viewModel.NoProjects);
        }

        [TestMethod]
        public void NoProjects_State_Changed_If_BoardProjectList_Is_Empty()
        {
            this._mockBoardList.Object.Values.Clear();

            this._viewModel = new ProjectListViewModel(_mockJiraToolWindowNavigatorViewModel.Object,
                this._mockProjectService.Object,
                this._mockBoardService.Object);

            Assert.AreEqual(this._viewModel.ProjectList.Count, 0);
            Assert.IsTrue(this._viewModel.NoProjects);
        }

        private void Add_Ten_BoardProjects()
        {
            for (int i = 0; i < NUMBER_OF_PROJECTS; i++)
            {
                this._mockBoardList.Object.Values.Add(_mockBoardProject.Object);
            }
        }
    }
}
