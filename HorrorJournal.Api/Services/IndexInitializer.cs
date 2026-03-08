using HorrorJournal.Api.Models;
using HorrorJournal.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HorrorJournal.Api.Services;

public class IndexInitializer : IHostedService
{
    private readonly IMongoClient _client;
    private readonly MongoDbSettings _settings;

    public IndexInitializer(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var database = _client.GetDatabase(_settings.DatabaseName);
        var collection = database.GetCollection<Film>("films");

        var indexes = new List<CreateIndexModel<Film>>
        {
            // Text index on title, director, notes for full-text search
            new(Builders<Film>.IndexKeys
                .Text(f => f.Title)
                .Text(f => f.Director)
                .Text(f => f.Notes),
                new CreateIndexOptions { Name = "text_title_director_notes" }),

            // Ascending index on status
            new(Builders<Film>.IndexKeys.Ascending(f => f.Status),
                new CreateIndexOptions { Name = "ix_status" }),

            // Ascending index on subgenres
            new(Builders<Film>.IndexKeys.Ascending(f => f.Subgenres),
                new CreateIndexOptions { Name = "ix_subgenres" }),

            // Ascending index on watchedOn
            new(Builders<Film>.IndexKeys.Ascending(f => f.WatchedOn),
                new CreateIndexOptions { Name = "ix_watchedOn" }),

            // Compound index on status + rating
            new(Builders<Film>.IndexKeys
                .Ascending(f => f.Status)
                .Ascending(f => f.Rating),
                new CreateIndexOptions { Name = "ix_status_rating" })
        };

        await collection.Indexes.CreateManyAsync(indexes, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
