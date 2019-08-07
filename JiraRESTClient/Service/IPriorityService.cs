using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{
    /// <summary>
    /// Interface providing methods to access resources connected to Priority object from Jira app.
    /// </summary>
    public interface IPriorityService
    {

        /// <summary>
        /// Async method to get a <see cref="List{Priority}"/> object containg all <see cref="Priority"/> related to currently authenticated user.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="List<Priority>"/> model class object.</returns>
        Task<List<Priority>> GetAllPrioritiesAsync();

    }
}
