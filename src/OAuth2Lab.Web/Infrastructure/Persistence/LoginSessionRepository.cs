using MongoDB.Driver;
using OAuth2Lab.Web.Shared.Models;

namespace OAuth2Lab.Web.Infrastructure.Persistence;

public class LoginSessionRepository(IMongoDatabase database)
{
    private readonly IMongoCollection<LoginSession> _collection =
        database.GetCollection<LoginSession>("login_sessions");

    public async Task<string> SaveAsync(LoginSession session)
    {
        await _collection.InsertOneAsync(session);
        return session.Id;
    }

    public async Task<LoginSession?> GetByIdAsync(string id)
        => await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();

    public async Task<List<LoginSession>> GetRecentAsync(int limit = 10)
        => await _collection.Find(_ => true)
            .SortByDescending(s => s.LoginAt)
            .Limit(limit)
            .ToListAsync();
}
