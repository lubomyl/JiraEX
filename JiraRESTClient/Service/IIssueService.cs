using DevDefined.OAuth.Framework;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
        Task<IssueListPaged> GetAllIssuesOfProjectAsync(int startAt, string projectKey);

        Task<IssueList> GetAllIssues();

        Task<IssueList> GetAllIssuesOfBoardOfSprintAsync(string boardId, string sprintId);

        Task<IssueList> GetAllIssuesOfBoardAsync(string boardId);

        Task UpdateIssuePropertyAsync(string issueKey, string action, string propertyName, object newValue);

        Task UpdateOriginalEstimatePropertyAsync(string issueKey, object newValue);

        Task<Issue> GetIssueByIssueKeyAsync(string issueKey);

        Task<EditableProperties> GetAllEditablePropertiesAsync(string issueKey);

        Task PostAttachmentToIssueAsync(FileInfo attachment, string issueKey);

        Task<Issue> CreateIssueAsync(string projectId, string summary, string description, string issueTypeId);

        Task<Issue> CreateSubTaskIssueAsync(string projectId, string summary, string description, string issueTypeId, string parentKey);

        Task AssignAsync(string issueKey, string userName);

        Task AddIssueVersionPropertyAsync(string issueKey, string versionType, object versionName);

        Task RemoveIssueVersionPropertyAsync(string issueKey, string versionType, object versionName);

        Task<LabelsList> GetAllLabelsAsync(string queryString);

        Task MoveIssueToSprintAsync(string issueKey, string sprintId);

        Task<List<Filter>> GetAllFiltersAsync();

        Task<IssueListPaged> GetAllIssuesByJqlAsync(int startAt, string jql);

        Task<IssueListPaged> GetAllIssuesByTextContainingAsync(int startAt, string quickSearch);

        Task<IssueLinkTypeList> GetAllIssueLinkTypes();

        Task LinkIssue(string inwardIssueKey, string outwardIssueKey, string linkName);

        Task DeleteLinkedIssue(string linkId);

        Task<IssueList> GetIssuesByIssueKeyAsync(string searchString);

        Task RemarkTimeSpentOnIssue(string timeSpent, string comment, string dateStarted, string issueKey);

        Task RemarkTimeRemainingOnIssue(string timeRemaining, string originalEstimate, string issueKey);
    }
}
