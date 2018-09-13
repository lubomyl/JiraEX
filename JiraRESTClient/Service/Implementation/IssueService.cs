using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Service;
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

        private JiraService _baseService;

        public IssueService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="IIssueService.GetAllIssuesAsync(string)"/>
        /// </summary>
        public Task<IssueList> GetAllIssuesOfProjectAsync(string projectKey)
        {
            return Task.Run(() => {
                var resource = $"search?jql=project={projectKey}";

                return this._baseService.GetResource<IssueList>(resource);
            });
        }

        public Task<IssueList> GetAllIssuesOfBoardOfSprintAsync(int boardId, int sprintId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/sprint/{sprintId}/issue";

                return this._baseService.GetAgileResource<IssueList>(resource);
            });
        }

        public Task<IssueList> GetAllIssuesOfBoardAsync(int boardId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/issue";

                return this._baseService.GetAgileResource<IssueList>(resource);
            });
        }

        public Task UpdateIssuePropertyAsync(string issueKey, string propertyName, object newValue)
        {
            return Task.Run(() => {
                newValue = JsonConvert.SerializeObject(newValue);

                string updateString = $"{{\"update\":{{\"{propertyName}\":[{{\"set\":{newValue}}}]}}}}";

                var resource = $"issue/{issueKey}";

                this._baseService.PutResource(resource, Encoding.UTF8.GetBytes(updateString));
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
    }
}
