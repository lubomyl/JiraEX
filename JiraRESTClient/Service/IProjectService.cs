using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{
    /// <summary>
    /// Interface providing methods to access resources connected to Project object from Jira app.
    /// </summary>
    public interface IProjectService
    {

        /// <summary>
        /// Async method to get a <see cref="List{Project}"/> object containg all <see cref="Project"/> related to currently authenticated user.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="ProjectList"/> model class object.</returns>
        Task<ProjectList> GetAllProjectsAsync();

        Task

    }
}
