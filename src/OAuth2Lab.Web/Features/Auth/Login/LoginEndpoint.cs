using Microsoft.AspNetCore.Authentication;

namespace OAuth2Lab.Web.Features.Auth.Login;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/login", async (HttpContext ctx, string? provider) =>
        {
            var scheme = provider switch
            {
                "Microsoft" => "Microsoft",
                "Google" => "Google",
                "GitHub" => "GitHub",
                _ => null
            };

            if (scheme is null)
            {
                ctx.Response.Redirect("/?error=provedor-invalido");
                return;
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = "/auth/callback"
            };

            await ctx.ChallengeAsync(scheme, properties);
        });
    }
}
