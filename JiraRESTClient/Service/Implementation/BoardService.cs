using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraRESTClient.Model;
using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Base;
using AtlassianConnector.Base.Implementation.RestSharp;

namespace JiraRESTClient.Service.Implementation
{

    /// <summary>
    /// Concrete implementation of IBoardService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IBoardService"/>
    /// </summary>
    public class BoardService : IBoardService
    {

        private IBaseJiraService _baseService;

        public BoardService(AuthenticationType type)
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
        /// <see cref="IBoardService.GetAllBoardsAsync"/>
        /// </summary>
        public Task<BoardProjectList> GetAllBoardsAsync()
        {
            return Task.Run(() => {
                var resource = "board";

                return this._baseService.GetAgileResource<BoardProjectList>(resource);
            });
        }

        public Task<BoardList> GetAllBoardsByProjectKeyAsync(string projectKey)
        {
            return Task.Run(() => {
                var resource = $"board?projectKeyOrId={projectKey}";

                return this._baseService.GetAgileResource<BoardList>(resource);
            });
        }

        public Task<SprintList> GetAllSprintsByBoardIdAsync(string boardId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/sprint";

                return this._baseService.GetAgileResource<SprintList>(resource);
            });
        }
    }
}
