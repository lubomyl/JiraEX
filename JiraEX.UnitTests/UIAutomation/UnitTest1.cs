using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White.UIItems.WindowItems;
using System.Collections.Generic;
using TestStack.White;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowStripControls;
using System.Windows.Automation;

namespace JiraEX.UnitTests.UIAutomation
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void UITest1()
        {
            Window window = Desktop.Instance.Windows().Find(w => w.Name.Equals("Start Page - Microsoft Visual Studio - Experimental Instance"));

            SearchCriteria searchCriteria = SearchCriteria
                .ByAutomationId("ToolbarTitle");

            SearchCriteria searchCriteria1 = SearchCriteria
                .ByText("Home")
                .AndByClassName("Button");

            Button b = (Button) window.Get(searchCriteria1);

            b.Click();

            Label textBlock = (Label) window.Get(searchCriteria);

            Assert.AreEqual("JiraEX", textBlock.Text);
        }
    }
}
