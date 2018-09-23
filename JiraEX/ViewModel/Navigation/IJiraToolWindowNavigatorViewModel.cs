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

        void ShowIssuesOfProject(BoardProject boardProject);

        void ShowIssueDetail(Issue issue, BoardProject project);

        void CreateIssue(BoardProject project);

        void CreateIssue(Issue parentIssue, BoardProject project);

        void SetPanelTitles(string title, string subtitle);

        void ShowIssuesOfFilter(Filter filter);
    }
}
