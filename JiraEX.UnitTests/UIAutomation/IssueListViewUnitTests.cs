using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White.UIItems.WindowItems;
using System.Collections.Generic;
using TestStack.White;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowStripControls;
using System.Windows.Automation;
using TestStack.White.UIItems.ListBoxItems;
using System.Windows;

namespace JiraEX.UnitTests.UIAutomation
{
    [TestClass]
    public class IssueListViewUnitTests
    {

        private TestStack.White.UIItems.WindowItems.Window _window;

        private Button _home, _back, _forward, _refresh, _connections, _filters, _advancedSearch;

        [TestInitialize]
        public void Initialize()
        {
            this._window = Desktop.Instance.Windows().Find(w => w.Name.Equals("JiraEX - Microsoft Visual Studio"));

            this._home = (Button)this._window.Get(SearchCriteria.ByText("Home").AndByClassName("Button"));
            this._back = (Button)this._window.Get(SearchCriteria.ByText("Back").AndByClassName("Button"));
            this._forward = (Button)this._window.Get(SearchCriteria.ByText("Forward").AndByClassName("Button"));
            this._refresh = (Button)this._window.Get(SearchCriteria.ByText("Refresh Issues").AndByClassName("Button"));
            this._connections = (Button)this._window.Get(SearchCriteria.ByText("Sign-in").AndByClassName("Button"));
            this._filters = (Button)this._window.Get(SearchCriteria.ByText("Filters").AndByClassName("Button"));
            this._advancedSearch = (Button)this._window.Get(SearchCriteria.ByText("Advanced Search").AndByClassName("Button"));

            this._home.Click();
        }

        [TestMethod]
        public void On_Attribute_Selection_Show_Or_Collapse()
        {
            bool isStatusVisible, isPriorityVisible, isTypeVisible, isCreatedVisible, isUpdatedVisible, isAssigneeVisible, isSummaryVisible;

            ListItem project = (ListItem)this._window.Get(SearchCriteria.ByClassName("ListBoxItem").AndByText("JiraRESTClient.Model.Project"));
            project.DoubleClick();

            ComboBox attributesComboBox = (ComboBox)this._window.Get(SearchCriteria.ByAutomationId("AttributesComboBox"));
            Label overlayTextBox = (Label)this._window.Get(SearchCriteria.ByAutomationId("AttributesOverlayTextBlock"));

            Label typeTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueType"));
            Label statusTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueStatus"));
            Label priorityTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssuePriority"));
            Label createdTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueCreated"));
            Label updatedTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueUpdated"));
            Label assigneeTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueAssignee"));
            Label summaryTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueSummary"));

            isTypeVisible = typeTextBlock.Visible;
            isStatusVisible = statusTextBlock.Visible;
            isPriorityVisible = priorityTextBlock.Visible;
            isCreatedVisible = createdTextBlock.Visible;
            isUpdatedVisible = updatedTextBlock.Visible;
            isAssigneeVisible = assigneeTextBlock.Visible;
            isSummaryVisible = summaryTextBlock.Visible;

            overlayTextBox.Click();

            ListItems attributes = attributesComboBox.Items;
            attributes[0].Click(); //Type
            attributes[1].Click(); //Status
            attributes[2].Click(); //Created
            attributes[3].Click(); //Updated
            attributes[4].Click(); //Assignee
            attributes[5].Click(); //Summary
            attributes[6].Click(); //Priority

            typeTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueType"));
            statusTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueStatus"));
            priorityTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssuePriority"));
            createdTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueCreated"));
            updatedTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueUpdated"));
            assigneeTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueAssignee"));
            summaryTextBlock = (Label)this._window.Get(SearchCriteria.ByAutomationId("IssueSummary"));

            if (isTypeVisible) Assert.AreEqual(false, typeTextBlock.Visible); else Assert.AreEqual(true, typeTextBlock.Visible);
            if (isStatusVisible) Assert.AreEqual(false, statusTextBlock.Visible); else Assert.AreEqual(true, statusTextBlock.Visible);
            if (isPriorityVisible) Assert.AreEqual(false, priorityTextBlock.Visible); else Assert.AreEqual(true, priorityTextBlock.Visible);
            if (isCreatedVisible) Assert.AreEqual(false, createdTextBlock.Visible); else Assert.AreEqual(true, createdTextBlock.Visible);
            if (isUpdatedVisible) Assert.AreEqual(false, updatedTextBlock.Visible); else Assert.AreEqual(true, updatedTextBlock.Visible);
            if (isAssigneeVisible) Assert.AreEqual(false, assigneeTextBlock.Visible); else Assert.AreEqual(true, assigneeTextBlock.Visible);
        }

    }
}
