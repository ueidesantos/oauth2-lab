using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OAuth2Lab.Web.Shared.Models;

public class LoginSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Provider { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime LoginAt { get; set; } = DateTime.UtcNow;
    public List<ClaimEntry> Claims { get; set; } = [];
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? TokenExpiration { get; set; }
    public List<string> Scopes { get; set; } = [];
}

public class ClaimEntry
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
