using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OAuth2Lab.Web.Shared.Extensions;
using OAuth2Lab.Web.Shared.Models;

namespace OAuth2Lab.Web.Features.TokenInspector;

[Authorize]
public class TokenInspectorModel : PageModel
{
    public UserClaimsViewModel? ViewModel { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded) return RedirectToPage("/Features/Home/Index");

        var principal = result.Principal!;
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

        var sessionId = HttpContext.Session.GetString("current_session_id") ?? string.Empty;

        ViewModel = new UserClaimsViewModel
        {
            Provider = principal.GetProvider(),
            DisplayName = principal.GetDisplayName(),
            Email = principal.GetEmail(),
            AvatarUrl = principal.GetAvatarUrl(),
            Claims = principal.ToClaimEntries(),
            SessionId = sessionId,
            Tokens = new OAuthTokenResult
            {
                AccessToken = accessToken,
                IdToken = idToken,
                RefreshToken = refreshToken,
                ExpiresAt = tokenExpiration,
                Scopes = scopes,
                AccessTokenDecoded = ClaimsExtensions.TryDecodeJwt(accessToken),
                IdTokenDecoded = ClaimsExtensions.TryDecodeJwt(idToken)
            }
        };

        return Page();
    }
}
