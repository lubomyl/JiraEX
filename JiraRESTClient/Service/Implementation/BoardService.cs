using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraRESTClient.Model;
using AtlassianConnector.Base.Implementation.DevDefined;

namespace JiraRESTClient.Service.Implementation
{
    public class BoardService : IBoardService
    {

        private BaseService _baseService;

        public BoardService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        public Task<BoardList> GetAllBoards()
        {
            return Task.Run(() => {
                var resource = "board";

                return this._baseService.GetAgile<BoardList>(resource);
            });
        }

    }
}
