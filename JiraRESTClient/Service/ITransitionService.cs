using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{

    /// <summary>
    /// Interface providing methods to access resources connected to Transition object from Jira app.
    /// </summary>
    public interface ITransitionService
    {

        /// <summary>
        /// Async method to get a <see cref="List{Transition}"/> object containg all <see cref="Transition"/> allowed to currently authenticated user for issue.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="TransitionList"/> model class object.</returns>
        Task<TransitionList> GetAllTransitionsForIssueByIssueKey(string issueKey);

        Task DoTransitionAsync(string issueKey, Transition selectedTransition);
    }
}
