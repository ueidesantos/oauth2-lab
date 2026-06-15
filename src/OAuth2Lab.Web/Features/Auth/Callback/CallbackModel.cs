using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OAuth2Lab.Web.Infrastructure.Persistence;
using OAuth2Lab.Web.Shared.Extensions;
using OAuth2Lab.Web.Shared.Models;

namespace OAuth2Lab.Web.Features.Auth.Callback;

public class CallbackModel(LoginSessionRepository repository) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal is null)
            return RedirectToPage("/Features/Auth/Error/AuthErrorPage",
                new { message = "Autenticação falhou ou foi cancelada." });

        var principal = result.Principal;
        var properties = result.Properties;

        var accessToken = properties?.GetTokenValue("access_token");
        var idToken = properties?.GetTokenValue("id_token");
        var refreshToken = properties?.GetTokenValue("refresh_token");
        var expiresAt = properties?.GetTokenValue("expires_at");

        DateTimeOffset? tokenExpiration = DateTimeOffset.TryParse(expiresAt, out var exp) ? exp : null;

        var scopesClaim = principal.FindFirst("scope")?.Value
            ?? principal.FindFirst("scp")?.Value
            ?? string.Empty;
        var scopes = scopesClaim.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        var session = new LoginSession
        {
            Provider = principal.GetProvider(),
            UserId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            Email = principal.GetEmail(),
            DisplayName = principal.GetDisplayName(),
            AvatarUrl = principal.GetAvatarUrl(),
            Claims = principal.ToClaimEntries(),
            AccessToken = accessToken,
            IdToken = idToken,
            RefreshToken = refreshToken,
            TokenExpiration = tokenExpiration,
            Scopes = scopes
        };

        var sessionId = await repository.SaveAsync(session);

        HttpContext.Session.SetString("current_session_id", sessionId);

        return RedirectToPage("/Features/TokenInspector/TokenInspectorPage");
    }
}
