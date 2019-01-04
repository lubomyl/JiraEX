using DevDefined.OAuth.Framework;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel.Navigation
{
    public interface IJiraToolWindowNavigatorViewModel
    {

        void InitializeServicesWithAuthenticationType(AuthenticationType type);

        void TryToSignIn();

        void ShowAuthentication();

        void ShowConnection(object sender, EventArgs e);

        void ShowAuthenticationVerification(object sender, EventArgs e, IToken requestToken);

        void ShowProjects(object sender, EventArgs e);

        void ShowIssuesOfProject(Project project);

        void ShowIssueDetail(Issue issue, Project project);

        void ShowCreateIssue(Project project);

        void ShowCreateIssue(Issue parentIssue, Project project);

        void SetPanelTitles(string title, string subtitle);

        void ShowIssuesOfFilter(Filter filter);

        void ShowIssuesQuickSearch(string quickSearch);

        void SetRefreshCommand(EventHandler command);

        void StartLoading();

        void StopLoading();

        void SetErrorMessage(string errorMessage);

        void ShowNoIssueFound(string issueKey);

        void DisposeConnectionView();

        void AreUserCredentialsCorrect();
    }
}
