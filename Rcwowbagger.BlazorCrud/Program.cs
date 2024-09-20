using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Radzen;
using Rcwowbagger.BlazorCrud.Auth;
using Rcwowbagger.BlazorCrud.Components;
using Rcwowbagger.BlazorCrud.DbPersistence;
using Rcwowbagger.BlazorCrud.Shared.Interfaces;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace Rcwowbagger.BlazorCrud
{
    public class Program
    {
        private const string MS_OIDC_SCHEME = "MicrosoftOidc";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            builder.Services
                .AddAuthentication(MS_OIDC_SCHEME)
                .AddOpenIdConnect(MS_OIDC_SCHEME, oidcOptions =>
                {
                    var adConfig = builder.Configuration.GetSection("AzureAd");
                    oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    oidcOptions.Authority = $"https://login.microsoftonline.com/{adConfig.GetValue<string>("TenantId")}/v2.0/";
                    oidcOptions.ClientId = adConfig.GetValue<string>("ClientId");
                    oidcOptions.ClientSecret = adConfig.GetValue<string>("ClientSecret");
                    oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
                    oidcOptions.MapInboundClaims = false;
                    oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                    oidcOptions.TokenValidationParameters.RoleClaimType = "role";
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, MS_OIDC_SCHEME);


            builder.Services
                .AddAuthorization(
                    policy => policy.FallbackPolicy = policy.DefaultPolicy);

            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddScoped<IDataRepository>((services) => new DatabaseRepo(services.GetService<IConfiguration>().GetSection("Database").Get<DatabaseSettings>()));
            builder.Services.AddRadzenComponents();

            builder.Services
                .AddServerSideBlazor();

            builder.Services
                .AddControllersWithViews();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                ;

            builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapGroup("/authentication").MapLoginAndLogout();

            app.Run();
        }
    }
}
