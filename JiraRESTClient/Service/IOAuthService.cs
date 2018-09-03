using DevDefined.OAuth.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Service
{

    /// <summary>
    /// Interface providing methods to (Re)Initialize OAuth session with all necessary parameters needed in HTTP request for successfull OAuth process.
    /// </summary>
    public interface IOAuthService
    {

        /// <summary>
        /// Method to initialize communication session.
        /// </summary>
        /// <param name="baseUrl">Base url used to make api calls. (e. g. https://confluencetest.atlassian.net)</param>
        void InitializeOAuthSession(string baseUrl);

        /// <summary>
        /// Method to reinitialize communication session with accessToken and accessTokenSecret. Used for rememberme-like function implementation.
        /// </summary>
        /// <param name="token">AccessToken obtained after successfull <see cref="ExchangeRequestTokenForAccessToken(IToken, string)"/> method call.</param>
        /// <param name="tokenSecret">AccessTokenSecret obtained after successfull <see cref="ExchangeRequestTokenForAccessToken(IToken, string)"/> method call.</param>
        /// <param name="baseUrl">Base url used to make api calls. (e. g. https://confluencetest.atlassian.net)</param>
        void ReinitializeOAuthSessionAccessToken(string token, string tokenSecret, string baseUrl);

        /// <summary>
        /// OAuth1.0 (STEP 1) - Async method to get temporary request token needed for further steps of authentication.
        /// </summary>
        /// <returns>Task containing <see cref="IToken"/> object containting oauth_token, oauth_token_secret strings.</returns>
        Task<IToken> GetRequestToken();

        /// <summary>
        /// OAuth1.0 (STEP 2) - Async method to get url needed to be presented to real user. Used for token authorization/access grant. 
        /// </summary>
        /// <returns>Task containing string representing url.</returns>
        /// <param name="requestToken"><see cref="IToken"/> object representing requestToken from STEP1. (<see cref="GetRequestToken"/>)</param>
        Task<string> GetUserAuthorizationUrlForToken(IToken requestToken);

        /// <summary>
        /// OAuth1.0 (STEP 3) - Async method to get accessToken in exchange for authorized reuqestToken from STEP1 (<see cref="GetRequestToken"/>) and oauth_verifier manually pasted by real user.
        /// </summary>
        /// <returns>Task containing <see cref="IToken"/> object representing aceessToken used for api calls for resources.</returns>
        /// <param name="requestToken"><see cref="IToken"/> object representing requestToken from STEP1. (<see cref="GetRequestToken"/>)</param>
        /// <param name="oAuthVerificationCode">Code generated on successfull STEP2 (<see cref="GetUserAuthorizationUrlForToken(IToken)"/>) verification. Provided manually by user.</param>
        Task<IToken> ExchangeRequestTokenForAccessToken(IToken requestToken, string oAuthVerificationCode);

    }
}
