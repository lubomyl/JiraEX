using JiraRESTClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{
    public interface IUserService
    {

        /// <summary>
        /// Async method to get <see cref="User"/> resources like username or displayName.
        /// </summary>
        /// <returns>Task containing deserialized <see cref="User"/> model class object.</returns>
        Task<User> GetAuthenticatedUserAsync();

    }
}
