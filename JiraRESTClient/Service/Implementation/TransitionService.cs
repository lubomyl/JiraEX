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
    /// Concrete implementation of ITransitionService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="ITransitionService"/>
    /// </summary>
    public class TransitionService : ITransitionService
    {

        private JiraService _baseService;

        public TransitionService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="ITransitionService.GetAllTransitionsForIssueByIssueKey(string)"/>
        /// </summary>
        public Task<TransitionList> GetAllTransitionsForIssueByIssueKey(string issueKey)
        {
            return Task.Run(() => {
                var resource = $"issue/{issueKey}/transitions";

                return this._baseService.GetResource<TransitionList>(resource);
            });
        }
    }
}
