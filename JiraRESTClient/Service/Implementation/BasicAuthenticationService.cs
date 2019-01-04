using AtlassianConnector.Base;
using AtlassianConnector.Base.Implementation.RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service.Implementation
{

    public class BasicAuthenticationService : IBasicAuthenticationService
    {

        private IBaseService _baseService;

        public BasicAuthenticationService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        public void InitializeBasicAuthenticationAuthenticator(string baseUrl, string username, string password)
        {
            ((BaseService)this._baseService).InitializeBasicAuthenticationAuthenticator(baseUrl, username, password);
        }

        public void DeleteAuthenticationCredentials()
        {
            ((BaseService)this._baseService).DeleteAuthenticationCredentials();
        }
    }
}
