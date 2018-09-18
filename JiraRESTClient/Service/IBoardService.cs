using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{

    /// <summary>
    /// Interface providing methods to access resources connected to Board object from Jira app.
    /// </summary>
    public interface IBoardService
    {

        /// <summary>
        /// Async method to get a <see cref="List{Board}"/> object containg all <see cref="Board"/> related to currently authenticated user.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="BoardList"/> model class object.</returns>
        Task<BoardList> GetAllBoardsAsync();

        Task<SprintList> GetAllSprintsByBoardIdAsync(string boardId);

    }
}
