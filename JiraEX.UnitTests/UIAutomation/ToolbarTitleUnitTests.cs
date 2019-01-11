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

namespace JiraEX.UnitTests.UIAutomation
{
    [TestClass]
    public class ToolbarTitleUnitTests
    {

        private Window _window;

        private Button _home, _back, _forward, _refresh, _connections, _filters, _advancedSearch;

        [TestInitialize]
        public void Initialize()
        {
            this._window = Desktop.Instance.Windows().Find(w => w.Name.Equals("JiraEX - Microsoft Visual Studio"));

            this._home = (Button) this._window.Get(SearchCriteria.ByText("Home").AndByClassName("Button"));
            this._back = (Button)this._window.Get(SearchCriteria.ByText("Back").AndByClassName("Button"));
            this._forward = (Button)this._window.Get(SearchCriteria.ByText("Forward").AndByClassName("Button"));
            this._refresh = (Button)this._window.Get(SearchCriteria.ByText("Refresh Issues").AndByClassName("Button"));
            this._connections = (Button)this._window.Get(SearchCriteria.ByText("Sign-in").AndByClassName("Button"));
            this._filters = (Button)this._window.Get(SearchCriteria.ByText("Filters").AndByClassName("Button"));
            this._advancedSearch = (Button)this._window.Get(SearchCriteria.ByText("Advanced Search").AndByClassName("Button"));

            this._home.Click();
        }

        [TestMethod]
        public void ToolbarTitle_Text_Equals_JiraEX()
        {
            SearchCriteria toolbarTitleTextSearchCriteria = SearchCriteria
                .ByAutomationId("ToolbarTitle");

            Label textBlock = (Label) this._window.Get(toolbarTitleTextSearchCriteria);

            Assert.AreEqual("JiraEX", textBlock.Text);
        }

        [TestMethod]
        public void ToolbarSubtitle_Text_Changes_By_View_Change()
        {
            SearchCriteria toolbarSubtitleTextSearchCriteria = SearchCriteria
                .ByAutomationId("ToolbarSubtitle");

            Label textBlock = (Label)this._window.Get(toolbarSubtitleTextSearchCriteria);
            Assert.AreEqual("Projects", textBlock.Text);

            this._connections.Click();
            textBlock = (Label)this._window.Get(toolbarSubtitleTextSearchCriteria);
            Assert.AreEqual("https://lubomyl12.atlassian.net", textBlock.Text);

            this._filters.Click();
            textBlock = (Label)this._window.Get(toolbarSubtitleTextSearchCriteria);
            Assert.AreEqual("Favourite filters", textBlock.Text);

            this._advancedSearch.Click();
            textBlock = (Label)this._window.Get(toolbarSubtitleTextSearchCriteria);
            Assert.AreEqual("Advanced search", textBlock.Text);
        }

        [TestMethod]
        public void ToolbarSubtitle_Text_Changes_By_Filter_Select()
        {
            this._filters.Click();

            SearchCriteria toolbarSubtitleTextSearchCriteria = SearchCriteria
                .ByAutomationId("ToolbarSubtitle");

            ListItem filter = (ListItem) this._window.Get(SearchCriteria.ByClassName("ListBoxItem").AndByText("JiraRESTClient.Model.Filter"));

            Label listBoxItemLabel = (Label) this._window.Get(SearchCriteria.ByAutomationId("JqlString"));

            filter.DoubleClick();

            Label subtitleLabel = (Label)this._window.Get(toolbarSubtitleTextSearchCriteria);

            Assert.AreEqual(subtitleLabel.Text, listBoxItemLabel.Text);
        }
    }
}
