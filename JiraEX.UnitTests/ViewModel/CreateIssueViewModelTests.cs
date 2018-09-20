using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraEX.ViewModel;
using Moq;
using JiraRESTClient.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JiraEX.ViewModel.Navigation;

namespace JiraEX.UnitTests.ViewModel
{
    [TestClass]
    public class CreateIssueViewModelTests
    {
        Mock<BoardProject> _mockBoardProject;
        Mock<IJiraToolWindowNavigatorViewModel> _mockJiraToolWindowNavigatorViewModel;
        Mock<Issue> _mockParent;

        CreateIssueViewModel _subTaskViewModel;
        CreateIssueViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            this._mockBoardProject = new Mock<BoardProject>();
            this._mockBoardProject.Object.CreatableIssueTypesList = new List<IssueType>();
            this._mockBoardProject.Object.CreatableIssueTypesList.Add(new IssueType());

            this._mockJiraToolWindowNavigatorViewModel = new Mock<IJiraToolWindowNavigatorViewModel>(It.IsAny<JiraToolWindow>());
            this._mockParent = new Mock<Issue>();

            this._mockJiraToolWindowNavigatorViewModel.Setup(mock => mock.ShowIssueDetail(It.IsAny<Issue>(), It.IsAny<BoardProject>())).Verifiable();

            this._subTaskViewModel = new CreateIssueViewModel(_mockJiraToolWindowNavigatorViewModel.Object, 
                _mockParent.Object, 
                _mockBoardProject.Object);
            this._viewModel = new CreateIssueViewModel(_mockJiraToolWindowNavigatorViewModel.Object, 
                _mockBoardProject.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void EnableEditType_Changes_IsEditingType_State()
        {
            Assert.IsFalse(this._viewModel.IsEditingType);

            this._viewModel.EditTypeCommand.Execute(null);

            Assert.IsTrue(this._viewModel.IsEditingType);
        }

        [TestMethod]
        public void CancelEditType_Changes_IsEditingType_State()
        {
            this._viewModel.IsEditingType = true;

            this._viewModel.CancelEditTypeCommand.Execute(null);

            Assert.IsFalse(this._viewModel.IsEditingType);
        }

        [TestMethod]
        public void EnableEditType_Doesnt_Change_IsEditingType_State_If_Subtask()
        {
            Assert.IsFalse(this._subTaskViewModel.IsEditingType);

            this._viewModel.EditTypeCommand.Execute(null);

            Assert.IsFalse(this._subTaskViewModel.IsEditingType);
        }

        [TestMethod]
        public void ShowIssueDetail_CalledOnce_On_Cancel_If_Creating_Subtask()
        {
            this._viewModel.CancelCreateIssueCommand.Execute(null);

            this._mockJiraToolWindowNavigatorViewModel.Verify(mock => mock.ShowIssueDetail(It.IsAny<Issue>(), It.IsAny<BoardProject>()));
        }


    }
}
