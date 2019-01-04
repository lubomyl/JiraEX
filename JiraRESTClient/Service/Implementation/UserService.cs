using AtlassianConnector.Base;
using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Base.Implementation.RestSharp;
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

        private IBaseJiraService _baseService;

        public UserService(AuthenticationType type)
        {
            if (type == AuthenticationType.Basic)
            {
                this._baseService = BaseService.JiraInstance;
            }
            else if (type == AuthenticationType.OAuth)
            {
                this._baseService = BaseOAuthService.JiraInstance;
            }
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

        public Task<UserList> GetAllAssignableUsersForIssueByIssueKey(string issueKey)
        {
            return Task.Run(() => {
                var resource = $"user/assignable/search?issueKey={issueKey}";

                return this._baseService.GetResource<UserList>(resource);
            });
        }

        public Task<UserList> GetAllAssignableUsersForIssueByIssueKeyAndByUsername(string issueKey, string username)
        {
            return Task.Run(() => {
                var resource = $"user/assignable/search?issueKey={issueKey}&username={username}";

                return this._baseService.GetResource<UserList>(resource);
            });
        }
    }
}
