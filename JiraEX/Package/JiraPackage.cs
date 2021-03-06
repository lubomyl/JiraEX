﻿//------------------------------------------------------------------------------
// <copyright file="JiraPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using JiraEX.Main;

namespace JiraEX
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.GUID_JIRA_PACKAGE_STRING)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideToolWindow(typeof(JiraToolWindow),
        Style = Microsoft.VisualStudio.Shell.VsDockStyle.Tabbed,
        Window = "3ae79031-e1bc-11d0-8f78-00a0c9110057")]
    [ProvideToolWindow(typeof(JiraWorklogToolWindow),
        Style = Microsoft.VisualStudio.Shell.VsDockStyle.AlwaysFloat,
        Width = 320,
        Height = 450,
        PositionX = 500,
        PositionY = 200,
        Transient = true)]
    public sealed class JiraPackage : Package
    {

        private static OleMenuCommandService _mcs;
        private static JiraWorklogToolWindow _jiraWorklogToolWindow;

        public static JiraWorklogToolWindow JiraWorklogToolWindowVar
        {
            get { return _jiraWorklogToolWindow; }
            private set { _jiraWorklogToolWindow = value; }
        }

        public static OleMenuCommandService Mcs
        {
            get { return _mcs; }
            private set { _mcs = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jira"/> class.
        /// </summary>
        public JiraPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        private void ShowJiraToolWindow(object sender, EventArgs e)
        {
            JiraToolWindow toolWindow = (JiraToolWindow)this.FindToolWindow(typeof(JiraToolWindow), 0, true);

            IVsWindowFrame windowFrame = (IVsWindowFrame)toolWindow.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            JiraWorklogToolWindowVar = (JiraWorklogToolWindow)this.FindToolWindow(typeof(JiraWorklogToolWindow), 0, true);

            _mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != _mcs)
            {
                CommandID menuCommandID = new CommandID(Guids.guidJiraCommand, Guids.JIRA_COMMAND_ID);

                MenuCommand onMenuCommandClickShowToolWindow = new MenuCommand(ShowJiraToolWindow, menuCommandID);

                _mcs.AddCommand(onMenuCommandClickShowToolWindow);
            }
        }

        #endregion
    }
}
