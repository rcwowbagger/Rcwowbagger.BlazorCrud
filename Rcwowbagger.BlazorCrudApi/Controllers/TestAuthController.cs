using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Rcwowbagger.BlazorCrudApi.Services;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Net;

namespace Rcwowbagger.BlazorCrudApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope("access_as_user")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class TestAuthController : ControllerBase
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly GraphServiceClient _graphServiceClient;
    private readonly IOptions<MicrosoftGraphOptions> _graphOptions;
    private readonly ClientHandlerService _service;

    private readonly ILogger<TestAuthController> _logger;

    public TestAuthController(ITokenAcquisition tokenAcquisition, GraphServiceClient graphServiceClient, IOptions<MicrosoftGraphOptions> graphOptions, ILogger<TestAuthController> logger)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        _graphServiceClient = graphServiceClient;
        _graphOptions = graphOptions;
    }

    [HttpGet(Name = "DoStuff")]
    public async Task<string> Get()
    {
        string owner = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        try
        {
            User user = await _graphServiceClient.Me.Request().GetAsync();
            //string title = string.IsNullOrWhiteSpace(user.UserPrincipalName) ? todo.Title : $"{todo.Title} ({user.UserPrincipalName})";
        }
        catch (MsalException ex)
        {
            HttpContext.Response.ContentType = "text/plain";
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await HttpContext.Response.WriteAsync("An authentication error occurred while acquiring a token for downstream API\n" + ex.ErrorCode + "\n" + ex.Message);
        }
        catch (Exception ex)
        {
            if (ex.InnerException is MicrosoftIdentityWebChallengeUserException challengeException)
            {
                _tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeader(_graphOptions.Value.Scopes.Split(' '),
                    challengeException.MsalUiRequiredException);
            }
            else
            {
                HttpContext.Response.ContentType = "text/plain";
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await HttpContext.Response.WriteAsync("An error occurred while calling the downstream API\n" + ex.Message);
            }
        }

        return string.Empty;
    }
}
