namespace OAuth2Lab.Web.Shared.Models;

public class OAuthTokenResult
{
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public List<string> Scopes { get; set; } = [];
    public string? AccessTokenDecoded { get; set; }
    public string? IdTokenDecoded { get; set; }
}
