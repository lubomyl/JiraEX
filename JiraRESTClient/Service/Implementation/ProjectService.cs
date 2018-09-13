using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraRESTClient.Model;
using DevDefined.OAuth.Framework;
using AtlassianConnector.Service;
using AtlassianConnector.Base.Implementation.DevDefined;

namespace JiraRESTClient.Service.Implementation
{

    /// <summary>
    /// Concrete implementation of IProjectService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IProjectService"/>
    /// </summary>
    public class ProjectService : IProjectService
    {

        private JiraService _baseService;

        public ProjectService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="IProjectService.GetAllProjectsAsync"/>
        /// </summary>
        public Task<ProjectList> GetAllProjectsAsync()
        {
            return Task.Run(() => {
                var resource = "project";

                return this._baseService.GetResource<ProjectList>(resource);
            });
        }

        public Task<ProjectCreatableList> GetAllProjectsCreatableIssueTypesAsync()
        {
            return Task.Run(() => {
                var resource = "issue/createmeta";

                return this._baseService.GetResource<ProjectCreatableList>(resource);
            });
        }
    }
}
