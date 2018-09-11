using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{

    /// <summary>
    /// Interface providing methods to access resources connected to Sprint object from Jira app.
    /// </summary>
    public interface ISprintService
    {

        /// <summary>
        /// Async method to get a <see cref="List{Sprint}"/> object containg all <see cref="Sprint"/> related to currently authenticated user.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="SprintList"/> model class object.</returns>
        Task<SprintList> GetAllSprintsOfBoardtAsync(int boardId);

    }
}
