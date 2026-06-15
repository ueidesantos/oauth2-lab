using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using OAuth2Lab.Web.Shared.Models;

namespace OAuth2Lab.Web.Shared.Extensions;

public static class ClaimsExtensions
{
    public static string GetProvider(this ClaimsPrincipal principal)
        => principal.FindFirst("provider")?.Value
        ?? principal.FindFirst("iss")?.Value
        ?? "Desconhecido";

    public static string GetDisplayName(this ClaimsPrincipal principal)
        => principal.FindFirst(ClaimTypes.Name)?.Value
        ?? principal.FindFirst("name")?.Value
        ?? principal.FindFirst("login")?.Value
        ?? "Usuário";

    public static string GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirst(ClaimTypes.Email)?.Value
        ?? principal.FindFirst("email")?.Value
        ?? string.Empty;

    public static string? GetAvatarUrl(this ClaimsPrincipal principal)
        => principal.FindFirst("urn:github:avatar")?.Value
        ?? principal.FindFirst("picture")?.Value;

    public static List<ClaimEntry> ToClaimEntries(this ClaimsPrincipal principal)
        => principal.Claims
            .Select(c => new ClaimEntry { Type = c.Type, Value = c.Value })
            .ToList();

    public static string? TryDecodeJwt(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2) return null;

            var payload = parts[1];
            var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return null;
        }
    }
}
