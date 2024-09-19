using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Rcwowbagger.BlazorCrudApi.Services;

namespace Rcwowbagger.BlazorCrudApi;

public static class AuthExtensions
{
    public static IServiceCollection AddProtectedApiCallsWebApis(this IServiceCollection services, IConfiguration configuration, IEnumerable<string> scopes)
    {

        var clientHandlerService = services.BuildServiceProvider().GetService<ClientHandlerService>();
        //...
        //services.Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
        //{
        //    options.Events.OnTokenValidated = async context =>
        //    {
        //        var tokenAcquisition = context.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();
        //        context.Success();

        //        clientHandlerService.
        //        // Adds the token to the cache, and also handles the incremental consent and claim challenges
        //        tokenAcquisition.AddAccountToCacheFromJwt(context, scopes);
        //        await Task.FromResult(0);
        //    };
        //});
        
        return services;
    }
}
