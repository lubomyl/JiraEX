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
    /// Concrete implementation of IBoardService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IBoardService"/>
    /// </summary>
    public class BoardService : IBoardService
    {

        private JiraService _baseService;

        public BoardService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="IBoardService.GetAllBoards"/>
        /// </summary>
        public Task<BoardList> GetAllBoards()
        {
            return Task.Run(() => {
                var resource = "board";

                return this._baseService.GetAgileResource<BoardList>(resource);
            });
        }

    }
}
