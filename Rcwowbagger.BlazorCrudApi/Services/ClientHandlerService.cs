using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Rcwowbagger.BlazorCrudApi.Services
{
    public class ClientHandlerService
    {
        private async Task AddAccountToCacheFromJwt(IEnumerable<string> scopes, JwtSecurityToken jwtToken, ClaimsPrincipal principal, HttpContext httpContext)
        {
            if (jwtToken == null)
            {
                throw new ArgumentOutOfRangeException("tokenValidationContext.SecurityToken should be a JWT Token");
            }
            UserAssertion userAssertion = new UserAssertion(jwtToken.RawData, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            IEnumerable<string> requestedScopes = scopes ?? jwtToken.Audiences.Select(a => $"{a}/.default");

            // Create the application
            var application = BuildConfidentialClientApplication(httpContext, principal);

            // await to make sure that the cache is filled in before the controller tries to get access tokens
            var result = await application.AcquireTokenOnBehalfOf(requestedScopes, userAssertion).ExecuteAsync();
        }

        // This example is for an ASP.NET Core web API
        public void ReplyUnauthorizedWithWwwAuthenticateHeader(HttpResponse httpResponse,  MsalUiRequiredException ex)
        {
            httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized; // HTTP 401
            httpResponse.Headers[HeaderNames.WWWAuthenticate] = $"Bearer claims={ex.Claims}, error={ex.Message}";
        }
    }
}
