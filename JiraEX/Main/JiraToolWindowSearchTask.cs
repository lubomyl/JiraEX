using AtlassianConnector.Model.Exceptions;
using JiraEX.View;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.Main
{
    public partial class JiraToolWindow : ToolWindowPane
    {
        private static IJiraToolWindowNavigatorViewModel navigator;

        private static IIssueService _issueService;

        public override bool SearchEnabled
        {
            get { return true; }
        }

        public override void ProvideSearchSettings(IVsUIDataSource pSearchSettings)
        {
            Utilities.SetValue(pSearchSettings,
                SearchSettingsDataSource.SearchWatermarkProperty.Name, "key:issueKey OR text");
        }

        public override IVsSearchTask CreateSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback)
        {
            if (pSearchQuery == null || pSearchCallback == null)
                return null;
            return new SearchTask(dwCookie, pSearchQuery, pSearchCallback, this);
        }

        public override void ClearSearch()
        {
        }

        internal class SearchTask : VsSearchTask
        {
            private JiraToolWindow m_toolWindow;

            public SearchTask(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback, JiraToolWindow toolwindow)
                : base(dwCookie, pSearchQuery, pSearchCallback)
            {
                m_toolWindow = toolwindow;

                _issueService = new IssueService(AuthenticationType.OAuth);
            }

            protected override void OnStartSearch()
            {
                JiraToolWindowNavigator control = (JiraToolWindowNavigator) m_toolWindow.Content;

                control.Dispatcher.Invoke(async () =>
                {
                    navigator = (IJiraToolWindowNavigatorViewModel)control.DataContext;

                    if (this.SearchQuery.SearchString.Length > 4 && this.SearchQuery.SearchString.Substring(0, 4).Equals("key:")){
                        Task<Issue> issueTask = _issueService.GetIssueByIssueKeyAsync(this.SearchQuery.SearchString.Substring(4));

                        try
                        {
                            Issue issue = await issueTask;

                            if (issue != null)
                            {
                                navigator.ShowIssueDetail(issue, null);
                            }
                        }
                        catch (JiraException ex)
                        {
                            navigator.ShowNoIssueFound(this.SearchQuery.SearchString.Substring(4));
                        }
                    }
                    else
                    {
                        navigator.ShowIssuesQuickSearch(this.SearchQuery.SearchString);
                    }
                });
               
                base.OnStartSearch();
            }

            protected override void OnStopSearch()
            {
                
            }
        }
    }
}
