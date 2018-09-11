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
    /// Concrete implementation of ISprintService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="ISprintService"/>
    /// </summary>
    public class SprintService : ISprintService
    {

        private JiraService _baseService;

        public SprintService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="ISprintService.GetAllSprintsOfBoardtAsync(int)"/>
        /// </summary>
        public Task<SprintList> GetAllSprintsOfBoardtAsync(int boardId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/sprint/";

                return this._baseService.GetAgileResource<SprintList>(resource);
            });
        }
    }
}
