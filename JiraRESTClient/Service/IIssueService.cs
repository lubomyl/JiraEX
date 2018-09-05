using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{
    /// <summary>
    /// Interface providing methods to access resources connected to Issue object from Jira app.
    /// </summary>
    public interface IIssueService
    {

        /// <summary>
        /// Async method to get a <see cref="List{Issue}"/> object containg all <see cref="Issue"/> related to currently authenticated user.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="IssueList"/> model class object.</returns>
        /// <param name="projectKey">Short string representing Jira Project.</param>
        Task<IssueList> GetAllIssuesOfProjectAsync(string projectKey);

    }
}
