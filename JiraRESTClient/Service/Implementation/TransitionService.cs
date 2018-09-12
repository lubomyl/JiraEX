using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraRESTClient.Model;
using AtlassianConnector.Base.Implementation.DevDefined;
using Newtonsoft.Json;

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


        public Task DoTransitionAsync(string issueKey, Transition selectedTransition)
        {
            return Task.Run(() => {
                var newValue = JsonConvert.SerializeObject(selectedTransition.Id);

                string transitionString = $"{{\"transition\":{{\"id\":{newValue}}}}}";

                var resource = $"issue/{issueKey}/transitions";

                this._baseService.PostResourceContent(resource, Encoding.UTF8.GetBytes(transitionString));
            });
        }
    }
}
