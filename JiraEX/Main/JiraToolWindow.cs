//------------------------------------------------------------------------------
// <copyright file="JiraToolWindow.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace JiraEX
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using ViewModel.Navigation;
    using View;

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
    [Guid("7953b9ea-8958-4cea-abe9-7cf58fc7d0c6")]
    public class JiraToolWindow : ToolWindowPane
    {
        private readonly object _view;
        private JiraToolWindowNavigatorViewModel _navigator;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public JiraToolWindow() : base(null)
        {
            this._navigator = new JiraToolWindowNavigatorViewModel(this);

            this._view = new JiraToolWindowNavigator(this._navigator);
            base.Content = _view;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
