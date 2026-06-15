using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OAuth2Lab.Web.Infrastructure.Auth;

public static class AuthenticationSetup
{
    public static IServiceCollection AddSsoAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/";
                options.LogoutPath = "/auth/logout";
                options.AccessDeniedPath = "/auth/error";
                options.Cookie.Name = "oauth2lab.session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            })
            .AddMicrosoftIdentityPlatform(configuration)
            .AddGoogleAuth(configuration)
            .AddGitHubAuth(configuration);

        return services;
    }

    private static AuthenticationBuilder AddMicrosoftIdentityPlatform(
        this AuthenticationBuilder builder,
        IConfiguration configuration)
    {
        var tenantId = configuration["Authentication:Microsoft:TenantId"];
        var clientId = configuration["Authentication:Microsoft:ClientId"];
        var clientSecret = configuration["Authentication:Microsoft:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId)) return builder;

        return builder.AddOpenIdConnect("Microsoft", "Microsoft", options =>
        {
            options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("offline_access");

            options.CallbackPath = "/signin-microsoft";

            options.Events.OnTokenValidated = ctx =>
            {
                ctx.Principal!.AddIdentity(new System.Security.Claims.ClaimsIdentity(
                [new System.Security.Claims.Claim("provider", "Microsoft")]));
                return Task.CompletedTask;
            };
        });
    }

    private static AuthenticationBuilder AddGoogleAuth(
        this AuthenticationBuilder builder,
        IConfiguration configuration)
    {
        var clientId = configuration["Authentication:Google:ClientId"];
        var clientSecret = configuration["Authentication:Google:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId)) return builder;

        return builder.AddGoogle(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret!;
            options.SaveTokens = true;
            options.CallbackPath = "/signin-google";

            options.Scope.Add("email");
            options.Scope.Add("profile");

            options.Events.OnCreatingTicket = ctx =>
            {
                ctx.Identity!.AddClaim(new System.Security.Claims.Claim("provider", "Google"));
                return Task.CompletedTask;
            };
        });
    }

    private static AuthenticationBuilder AddGitHubAuth(
        this AuthenticationBuilder builder,
        IConfiguration configuration)
    {
        var clientId = configuration["Authentication:GitHub:ClientId"];
        var clientSecret = configuration["Authentication:GitHub:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId)) return builder;

        return builder.AddGitHub(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret!;
            options.SaveTokens = true;
            options.CallbackPath = "/signin-github";

            options.Scope.Add("read:user");
            options.Scope.Add("user:email");

            options.Events.OnCreatingTicket = ctx =>
            {
                ctx.Identity!.AddClaim(new System.Security.Claims.Claim("provider", "GitHub"));
                return Task.CompletedTask;
            };
        });
    }
}
