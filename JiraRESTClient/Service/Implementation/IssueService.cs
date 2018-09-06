using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Service;
using DevDefined.OAuth.Framework;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
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

        private IBaseService<IToken> _baseService;

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

                return this._baseService.Get<IssueList>(resource);
            });
        }

        public Task<IssueList> GetAllIssuesOfBoardOfSprintAsync(int boardId, int sprintId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/sprint/{sprintId}/issue";

                return this._baseService.Get<IssueList>(resource);
            });
        }

    }
}
