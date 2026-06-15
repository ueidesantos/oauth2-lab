namespace OAuth2Lab.Web.Shared.Models;

public class UserClaimsViewModel
{
    public string Provider { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public List<ClaimEntry> Claims { get; set; } = [];
    public OAuthTokenResult Tokens { get; set; } = new();
    public string SessionId { get; set; } = string.Empty;
}
