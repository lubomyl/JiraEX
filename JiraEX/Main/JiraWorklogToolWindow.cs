//------------------------------------------------------------------------------
// <copyright file="JiraToolWindow.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace JiraEX.Main
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using ViewModel.Navigation;
    using View;
    using Main;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Windows.Forms;
    using JiraRESTClient.Model;
    using JiraEX.View.Navigation;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid(Guids.GUID_JIRA_WORKLOG_TOOL_WINDOW_STRING)]
    public class JiraWorklogToolWindow : ToolWindowPane
    {
        private object _view;
        private RefreshableWorklogViewModel _viewModel;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public JiraWorklogToolWindow() : base(null)
        {
            this._viewModel = new RefreshableWorklogViewModel(this);
            this._view = new RefreshableWorklogView(this._viewModel);

            base.Content = _view;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public RefreshableWorklogViewModel ViewModel
        {
            get { return this._viewModel; }
            private set { }
        }
    }
}
