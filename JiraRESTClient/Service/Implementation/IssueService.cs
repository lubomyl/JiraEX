using AtlassianConnector.Base;
using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Base.Implementation.RestSharp;
using AtlassianConnector.Model.Exceptions;
using DevDefined.OAuth.Framework;
using JiraRESTClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service.Implementation
{
    /// <summary>
    /// Concrete implementation of IIssueService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IIssueService"/>
    /// </summary>
    public class IssueService : IIssueService
    {

        private IBaseJiraService _baseService;

        public IssueService(AuthenticationType type)
        {
            if (type == AuthenticationType.Basic)
            {
                this._baseService = BaseService.JiraInstance;
            }
            else if (type == AuthenticationType.OAuth)
            {
                this._baseService = BaseOAuthService.JiraInstance;
            }
        }

        /// <summary>
        /// <see cref="IIssueService.GetAllIssuesAsync(string)"/>
        /// </summary>
        public Task<IssueListPaged> GetAllIssuesOfProjectAsync(int startAt, string projectKey)
        {
            return Task.Run(() => {
                var resource = $"search?jql=project={projectKey}&startAt={startAt}";

                return this._baseService.GetResource<IssueListPaged>(resource);
            });
        }

        public Task<IssueList> GetAllIssuesOfBoardOfSprintAsync(string boardId, string sprintId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/sprint/{sprintId}/issue";

                return this._baseService.GetAgileResource<IssueList>(resource);
            });
        }

        public Task<IssueList> GetAllIssuesOfBoardAsync(string boardId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/issue";

                return this._baseService.GetAgileResource<IssueList>(resource);
            });
        }

        public Task UpdateIssuePropertyAsync(string issueKey, string action, string propertyName, object newValue)
        {
            return Task.Run(() => {
                newValue = JsonConvert.SerializeObject(newValue);

                string updateString = $"{{\"update\":{{\"{propertyName}\":[{{\"{action}\":{newValue}}}]}}}}";

                var resource = $"issue/{issueKey}";

                this._baseService.PutResource(resource, updateString);
            });
        }

        public Task UpdateOriginalEstimatePropertyAsync(string issueKey, object newValue)
        {
            return Task.Run(() => {
                newValue = JsonConvert.SerializeObject(newValue);

                string updateString = $"{{\"update\":{{\"timetracking\":[{{\"edit\": {{\"originalEstimate\":{newValue}}}}}]}}}}";

                var resource = $"issue/{issueKey}";

                this._baseService.PutResource(resource, updateString);
            });
        }

        public Task<Issue> GetIssueByIssueKeyAsync(string issueKey)
        {
            return Task.Run(() => {
                var resource = $"issue/{issueKey}";

                return this._baseService.GetAgileResource<Issue>(resource);
            });
        }

        public Task<EditableProperties> GetAllEditablePropertiesAsync(string issueKey)
        {
            return Task.Run(() => {
                var resource = $"issue/{issueKey}/editmeta";

                return this._baseService.GetResource<EditableProperties>(resource);
            });
        }

        public Task PostAttachmentToIssueAsync(FileInfo attachment, string issueKey)
        {
            return Task.Run(() => {
                var resource = $"issue/{issueKey}/attachments";

                this._baseService.PostResourceFile(resource, attachment);
            });
        }

        public Task<Issue> CreateIssueAsync(string projectId, string summary, string description, string issueTypeId)
        {
            return Task.Run(() => {
                string createString = "{\"fields\":" +
                                            "{" + 
                                                "\"project\": { " +
                                                    $"\"id\":\"{projectId}\"" +
                                                "}," +
                                                 $"\"summary\":\"{summary}\"," +
                                                 $"\"description\":\"{description}\"," +
                                                 "\"issuetype\": { " +
                                                    $"\"id\":\"{issueTypeId}\"" +
                                                 "}" +
                                             "}" +
                                       "}";


                var resource = "issue";

                return this._baseService.PostResourceContentWithResponse<Issue>(resource, createString);
            });
        }

        public Task<Issue> CreateSubTaskIssueAsync(string projectId, string summary, string description, string issueTypeId, string parentKey)
        {
            return Task.Run(() => {
                string createString = "{\"fields\":" +
                                            "{" +
                                                "\"project\": { " +
                                                    $"\"id\":\"{projectId}\"" +
                                                "}," +
                                                "\"parent\": { " +
                                                    $"\"key\":\"{parentKey}\"" +
                                                "}," +
                                                 $"\"summary\":\"{summary}\"," +
                                                 $"\"description\":\"{description}\"," +
                                                 "\"issuetype\": { " +
                                                    $"\"id\":\"{issueTypeId}\"" +
                                                 "}" +
                                             "}" +
                                       "}";


                var resource = "issue";

                return this._baseService.PostResourceContentWithResponse<Issue>(resource, createString);
            });
        }

        public Task AssignAsync(string issueKey, string userName)
        {
            return Task.Run(() => {
                string updateString = $"{{\"name\":\"{userName}\"}}";

                var resource = $"issue/{issueKey}/assignee";

                this._baseService.PutResource(resource, updateString);
            });
        }

        public Task AddIssueVersionPropertyAsync(string issueKey, string versionType, object versionName)
        {
            return Task.Run(() => {
                versionName = JsonConvert.SerializeObject(versionName);

                string updateString = $"{{\"update\":{{\"{versionType}\":[{{\"add\":{{\"name\":{versionName}}}}}]}}}}";

                var resource = $"issue/{issueKey}";

                this._baseService.PutResource(resource, updateString);
            });
        }

        public Task RemoveIssueVersionPropertyAsync(string issueKey, string versionType, object versionName)
        {
            return Task.Run(() => {
                versionName = JsonConvert.SerializeObject(versionName);

                string updateString = $"{{\"update\":{{\"{versionType}\":[{{\"remove\":{{\"name\":{versionName}}}}}]}}}}";

                var resource = $"issue/{issueKey}";

                this._baseService.PutResource(resource, updateString);
            });
        }

        public Task<LabelsList> GetAllLabelsAsync(string queryString)
        {
            return Task.Run(() => {
                var resource = $"labels/suggest?query={queryString}";

                return this._baseService.Get10Resource<LabelsList>(resource);
            });
        }

        public Task MoveIssueToSprintAsync(string issueKey, string sprintId)
        {
            return Task.Run(() => {
                string updateString = $"{{\"issues\":[\"{issueKey}\"]}}";

                var resource = $"sprint/{sprintId}/issue";

                this._baseService.PostAgileResourceContent(resource, updateString);
            });
        }

        Task<FilterList> IIssueService.GetAllFiltersAsync()
        {
            return Task.Run(() => {
                var resource = "filter/favourite";

                return this._baseService.GetResource<FilterList>(resource);
            });
        }

        public Task<IssueListPaged> GetAllIssuesByJqlAsync(int startAt, string jql)
        {
            return Task.Run(() => {
                var resource = $"search?jql={jql}&startAt={startAt}";

                return this._baseService.GetResource<IssueListPaged>(resource);
            });
        }

        public Task<IssueListPaged> GetAllIssuesByTextContainingAsync(int startAt, string quickSearch)
        {
            return Task.Run(() => {
                var resource = $"search?jql=text~\"{quickSearch}*\"&startAt={startAt}";

                return this._baseService.GetResource<IssueListPaged>(resource);
            });
        }

        public Task<IssueList> GetAllIssues()
        {
            return Task.Run(() => {
                var resource = $"search";

                return this._baseService.GetResource<IssueList>(resource);
            });
        }

        public Task<IssueLinkTypeList> GetAllIssueLinkTypes()
        {
            return Task.Run(() => {
                var resource = $"issueLinkType";

                return this._baseService.GetResource<IssueLinkTypeList>(resource);
            });
        }

        public Task LinkIssue(string inwardIssueKey, string outwardIssueKey, string linkName)
        {
            return Task.Run(() => {
                string linkString = "{" +
                                            "\"type\": { " +
                                                $"\"name\":\"{linkName}\"" +
                                            "}," +
                                            "\"inwardIssue\": { " +
                                                $"\"key\":\"{inwardIssueKey}\"" +
                                            "}," +
                                            "\"outwardIssue\": { " +
                                                $"\"key\":\"{outwardIssueKey}\"" +
                                            "}" +
                                    "}";


                var resource = "issueLink";

                this._baseService.PostResourceContent(resource, linkString);
            });
        }

        public Task DeleteLinkedIssue(string linkId)
        {
            return Task.Run(() => {
                var resource = $"issueLink/{linkId}";

                this._baseService.DeleteResource(resource);
            });
        }

        public Task<IssueList> GetIssuesByIssueKeyAsync(string searchString)
        {
            return Task.Run(() =>
            {
                var resource = $"search?jql=key = " + searchString;

                return this._baseService.GetResource<IssueList>(resource);
            });
        }
    }
}
