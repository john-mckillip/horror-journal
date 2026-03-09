using HorrorJournal.Api.Models;
using HorrorJournal.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HorrorJournal.Api.Services;

public class FilmChangeStreamService(
    IMongoClient client,
    IOptions<MongoDbSettings> settings,
    ILogger<FilmChangeStreamService> logger) : BackgroundService
{
    private readonly IMongoClient _client = client;
    private readonly MongoDbSettings _settings = settings.Value;
    private readonly ILogger<FilmChangeStreamService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var database = _client.GetDatabase(_settings.DatabaseName);
        var collection = database.GetCollection<Film>("films");

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Film>>()
            .Match(change =>
                change.OperationType == ChangeStreamOperationType.Insert ||
                change.OperationType == ChangeStreamOperationType.Update ||
                change.OperationType == ChangeStreamOperationType.Replace);

        using var cursor = await collection.WatchAsync(pipeline, cancellationToken: stoppingToken);

        _logger.LogInformation("Film change stream started");

        while (await cursor.MoveNextAsync(stoppingToken))
        {
            foreach (var change in cursor.Current)
            {
                stoppingToken.ThrowIfCancellationRequested();

                var title = change.FullDocument?.Title ?? "unknown";
                var operation = change.OperationType.ToString();
                var timestamp = change.ClusterTime?.Timestamp ?? 0;

                _logger.LogInformation(
                    "Change detected: {Operation} on \"{Title}\" at {Timestamp}",
                    operation, title, DateTimeOffset.FromUnixTimeSeconds(timestamp));
            }
        }
    }
}
