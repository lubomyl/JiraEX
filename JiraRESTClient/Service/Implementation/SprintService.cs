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
    /// Concrete implementation of ISprintService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="ISprintService"/>
    /// </summary>
    public class SprintService : ISprintService
    {

        private IBaseJiraService _baseService;

        public SprintService(AuthenticationType type)
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
        /// <see cref="ISprintService.GetAllSprintsOfBoardtAsync(string)"/>
        /// </summary>
        public Task<SprintList> GetAllSprintsOfBoardtAsync(string boardId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/sprint/";

                return this._baseService.GetAgileResource<SprintList>(resource);
            });
        }
    }
}
