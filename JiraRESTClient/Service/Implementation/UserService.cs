using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Service;
using DevDefined.OAuth.Framework;
using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service.Implementation
{
    public class UserService : IUserService
    {

        private JiraService _baseService;

        public UserService()
        {
            _baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="IUserService.GetAuthenticatedUserAsync"/>
        /// </summary>
        public Task<User> GetAuthenticatedUserAsync()
        {
            return Task.Run(() => {
                var resource = "myself";

                return this._baseService.GetResource<User>(resource);
            });
        }

    }
}
