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
    [Guid(Guids.GUID_JIRA_TOOL_WINDOW_STRING)]
    public partial class JiraToolWindow : ToolWindowPane
    {
        private readonly object _view;
        private JiraToolWindowNavigatorViewModel _navigator;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public JiraToolWindow() : base(null)
        {
            this.Caption = Resources.JiraToolWindowTitle;
            this._navigator = new JiraToolWindowNavigatorViewModel(this);

            this._view = new JiraToolWindowNavigator(this._navigator);
            base.Content = _view;

            this.ToolBar = new CommandID(Guids.guidJiraPackage, Guids.JIRA_TOOLBAR_ID);

            this._navigator.TryToSignIn(null, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
