using AtlassianConnector.Base.Implementation.DevDefined;
using AtlassianConnector.Service;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service.Implementation
{

    /// <summary>
    /// Concrete implementation of IOAuthService utilizing <see cref="BaseService"/> as <see cref="IBaseService{T}"/>.
    /// <see cref="IContentService"/>
    /// </summary>
    public class OAuthService : IOAuthService
    {
        private IBaseService<IToken> _baseService;

        public OAuthService()
        {
            this._baseService = BaseService.JiraInstance;
        }

        /// <summary>
        /// <see cref="IOAuthService.InitializeOAuthSession"/>
        /// </summary>
        public void InitializeOAuthSession(string baseUrl)
        {
            ((BaseService)this._baseService).InitializeOAuthSession(baseUrl);
        }

        /// <summary>
        /// <see cref="IOAuthService.ReinitializeOAuthSessionAccessToken(string, string, string)"/>
        /// </summary>
        public void ReinitializeOAuthSessionAccessToken(string token, string tokenSecret, string baseUrl)
        {
            ((BaseService)this._baseService).ReinitializeOAuthSessionAccessToken(token, tokenSecret, baseUrl);
        }

        /// <summary>
        /// <see cref="IOAuthService.GetRequestToken"/>
        /// </summary>
        public Task<IToken> GetRequestToken()
        {
            return Task.Run(() =>
            {
                IToken requestToken = this._baseService.GetRequestToken();

                return requestToken;
            });
        }

        /// <summary>
        /// <see cref="IOAuthService.GetUserAuthorizationUrlForToken(IToken)"/>
        /// </summary>
        public Task<string> GetUserAuthorizationUrlForToken(IToken requestToken)
        {
            return Task.Run(() => {
                string authorizationUrl = this._baseService.GetUserAuthorizationUrlForToken(requestToken);
                return authorizationUrl;
            });
        }

        /// <summary>
        /// <see cref="IOAuthService.ExchangeRequestTokenForAccessToken(IToken, string)"/>
        /// </summary>
        public Task<IToken> ExchangeRequestTokenForAccessToken(IToken requestToken, string oAuthVerificationCode)
        {
            return Task.Run(() =>
            {
                IToken accessToken = this._baseService.ExchangeRequestTokenForAccessToken(requestToken, oAuthVerificationCode);
                return accessToken;
            });
        }
    }
}
