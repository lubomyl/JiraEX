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
    /// Concrete implementation of IPriorityService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IPriorityService"/>
    /// </summary>
    public class PriorityService : IPriorityService
    {

        private IBaseJiraService _baseService;

        public PriorityService(AuthenticationType type)
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
        /// <see cref="IPriorityService.GetAllPrioritiesAsync"/>
        /// </summary>
        public Task<List<Priority>> GetAllPrioritiesAsync()
        {
            return Task.Run(() => {
                var resource = "priority";

                return this._baseService.GetResource<List<Priority>>(resource);
            });
        }

    }
}
