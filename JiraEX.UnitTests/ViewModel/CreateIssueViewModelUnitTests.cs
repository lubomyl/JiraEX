using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraEX.ViewModel;
using Moq;
using JiraRESTClient.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Service;

namespace JiraEX.UnitTests.ViewModel
{
    [TestClass]
    public class CreateIssueViewModelUnitTests
    {
        Mock<Project> _mockProject;
        Mock<IJiraToolWindowNavigatorViewModel> _mockJiraToolWindowNavigatorViewModel;
        Mock<IIssueService> _mockIssueService;
        Mock<Issue> _mockParent;

        CreateIssueViewModel _subTaskViewModel;
        CreateIssueViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            this._mockProject = new Mock<Project>();

            //Cannot mock Get methods on Class object - only works against Interface with Moq framework
            this._mockProject.Object.CreatableIssueTypesList = new List<IssueType>();
            this._mockProject.Object.CreatableIssueTypesList.Add(new IssueType());

            this._mockJiraToolWindowNavigatorViewModel = new Mock<IJiraToolWindowNavigatorViewModel>();
            this._mockIssueService = new Mock<IIssueService>();
            this._mockParent = new Mock<Issue>();

            this._mockJiraToolWindowNavigatorViewModel.Setup(mock => mock.ShowIssueDetail(It.IsAny<Issue>(), 
                It.IsAny<Project>())).Verifiable();
            this._mockIssueService.Setup(mock => mock.CreateIssueAsync(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            this._mockIssueService.Setup(mock => mock.CreateSubTaskIssueAsync(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            this._subTaskViewModel = new CreateIssueViewModel(_mockJiraToolWindowNavigatorViewModel.Object, 
                _mockParent.Object, 
                _mockProject.Object,
                this._mockIssueService.Object);
            this._viewModel = new CreateIssueViewModel(_mockJiraToolWindowNavigatorViewModel.Object, 
                _mockProject.Object,
                this._mockIssueService.Object);

            this._subTaskViewModel.SelectedType = new Mock<IssueType>().Object;
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
            this._subTaskViewModel.CancelCreateIssueCommand.Execute(null);

            this._mockJiraToolWindowNavigatorViewModel.Verify(mock => 
                mock.ShowIssueDetail(It.IsAny<Issue>(), It.IsAny<Project>()), Times.Once());
        }

        [TestMethod]
        public void ShowIssuesOfProject_CalledOnce_On_Cancel_If_Creating_NewIssue()
        {
            this._viewModel.CancelCreateIssueCommand.Execute(null);

            this._mockJiraToolWindowNavigatorViewModel.Verify(mock =>
                mock.ShowIssuesOfProject(It.IsAny<Project>()), Times.Once());
        }

        [TestMethod]
        public void CreateSubTaskIssue_CalledOnce_On_Create_If_Creating_Subtask()
        {
            this._subTaskViewModel.ConfirmCreateIssueCommand.Execute(null);

            this._mockIssueService.Verify(mock =>
                mock.CreateSubTaskIssueAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void CreateIssue_CalledOnce_On_Create_If_Creating_NewIssue()
        {
            this._viewModel.ConfirmCreateIssueCommand.Execute(null);

            this._mockIssueService.Verify(mock =>
                mock.CreateIssueAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Once());
        }

    }
}
