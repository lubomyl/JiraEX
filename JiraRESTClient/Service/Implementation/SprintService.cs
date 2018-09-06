using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraRESTClient.Model;
using AtlassianConnector.Base.Implementation.DevDefined;

namespace JiraRESTClient.Service.Implementation
{
    public class SprintService : ISprintService
    {

        private BaseService _baseService;

        public SprintService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        public Task<SprintList> GetAllSprintsOfBoardtAsync(int boardId)
        {
            return Task.Run(() => {
                var resource = $"board/{boardId}/sprint/";

                return this._baseService.GetAgile<SprintList>(resource);
            });
        }
    }
}
