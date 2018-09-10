using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraRESTClient.Model;
using AtlassianConnector.Base.Implementation.DevDefined;

namespace JiraRESTClient.Service.Implementation
{

    /// <summary>
    /// Concrete implementation of IPriorityService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IPriorityService"/>
    /// </summary>
    public class PriorityService : IPriorityService
    {

        private JiraService _baseService;

        public PriorityService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="IPriorityService.GetAllPrioritiesAsync"/>
        /// </summary>
        public Task<PriorityList> GetAllPrioritiesAsync()
        {
            return Task.Run(() => {
                var resource = "priority";

                return this._baseService.GetResource<PriorityList>(resource);
            });
        }

    }
}
