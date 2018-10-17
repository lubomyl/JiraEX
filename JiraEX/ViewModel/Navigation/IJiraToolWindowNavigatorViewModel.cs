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

        void ShowConnection(object sender, EventArgs e);

        void ShowBeforeSignIn();

        void ShowAfterSignIn();

        void ShowOAuthVerificationConfirmation(object sender, EventArgs e, IToken requestToken);

        void ShowProjects(object sender, EventArgs e);

        void ShowIssuesOfProject(Project project);

        void ShowIssueDetail(Issue issue, Project project);

        void CreateIssue(Project project);

        void CreateIssue(Issue parentIssue, Project project);

        void SetPanelTitles(string title, string subtitle);

        void ShowIssuesOfFilter(Filter filter);

        void StartLoading();

        void StopLoading();
    }
}
