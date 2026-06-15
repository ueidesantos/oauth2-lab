using MongoDB.Driver;

namespace OAuth2Lab.Web.Infrastructure.Persistence;

public static class MongoDbSetup
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"]
            ?? throw new InvalidOperationException("MongoDB:ConnectionString não configurado.");
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "oauth2lab";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        services.AddSingleton(database);
        services.AddSingleton<LoginSessionRepository>();

        return services;
    }
}
