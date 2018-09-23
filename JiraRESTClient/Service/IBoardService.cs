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
        /// Async method to get a <see cref="List{Board}"/> object containg all <see cref="BoardList"/> related to currently authenticated user.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="BoardProjectList"/> model class object.</returns>
        Task<BoardProjectList> GetAllBoardsAsync();

        Task<SprintList> GetAllSprintsByBoardIdAsync(string boardId);

        Task<BoardList> GetAllBoardsByProjectKeyAsync(string projectKey);
    }
}
