using HorrorJournal.Api.Models;
using HorrorJournal.Api.Models.Requests;
using Microsoft.Extensions.Options;
using HorrorJournal.Api.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HorrorJournal.Api.Repositories;

public class FilmRepository : IFilmRepository
{
    private readonly IMongoCollection<Film> _films;

    public FilmRepository(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _films = database.GetCollection<Film>("films");
    }

    public async Task<List<Film>> GetAllAsync(FilmFilter filter)
    {
        var builder = Builders<Film>.Filter;
        var filters = new List<FilterDefinition<Film>>();

        if (filter.Status.HasValue)
            filters.Add(builder.Eq(f => f.Status, filter.Status.Value));

        if (!string.IsNullOrWhiteSpace(filter.Subgenre))
            filters.Add(builder.AnyEq(f => f.Subgenres, filter.Subgenre));

        if (filter.MinRating.HasValue)
            filters.Add(builder.Gte(f => f.Rating, filter.MinRating.Value));

        var combined = filters.Count > 0
            ? builder.And(filters)
            : builder.Empty;

        return await _films.Find(combined).ToListAsync();
    }

    public async Task<Film?> GetByIdAsync(string id)
    {
        return await _films.Find(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Film>> SearchAsync(string text)
    {
        var filter = Builders<Film>.Filter.Or(
            Builders<Film>.Filter.Regex(f => f.Title, new BsonRegularExpression(text, "i")),
            Builders<Film>.Filter.Regex(f => f.Director, new BsonRegularExpression(text, "i")),
            Builders<Film>.Filter.Regex(f => f.Notes, new BsonRegularExpression(text, "i"))
        );

        return await _films.Find(filter).ToListAsync();
    }

    public async Task InsertAsync(Film film)
    {
        var now = DateTime.UtcNow;
        film.CreatedAt = now;
        film.UpdatedAt = now;
        await _films.InsertOneAsync(film);
    }

    public async Task<bool> UpdateAsync(string id, FilmUpdate update)
    {
        var builder = Builders<Film>.Update;
        var updates = new List<UpdateDefinition<Film>>
        {
            builder.Set(f => f.UpdatedAt, DateTime.UtcNow)
        };

        if (update.Title is not null)
            updates.Add(builder.Set(f => f.Title, update.Title));

        if (update.Year.HasValue)
            updates.Add(builder.Set(f => f.Year, update.Year.Value));

        if (update.Director is not null)
            updates.Add(builder.Set(f => f.Director, update.Director));

        if (update.Subgenres is not null)
            updates.Add(builder.Set(f => f.Subgenres, update.Subgenres));

        if (update.Status.HasValue)
            updates.Add(builder.Set(f => f.Status, update.Status.Value));

        if (update.Rating.HasValue)
            updates.Add(builder.Set(f => f.Rating, update.Rating.Value));

        if (update.WatchedOn.HasValue)
            updates.Add(builder.Set(f => f.WatchedOn, update.WatchedOn.Value));

        if (update.Notes is not null)
            updates.Add(builder.Set(f => f.Notes, update.Notes));

        var result = await _films.UpdateOneAsync(
            f => f.Id == id,
            builder.Combine(updates));

        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _films.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<FilmStats> GetStatsAsync()
    {
        var stats = new FilmStats();

        // Count by status
        var statusCounts = await _films.Aggregate()
            .Group(f => f.Status, g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        foreach (var sc in statusCounts)
            stats.CountByStatus[sc.Status] = sc.Count;

        // Average rating (only rated films)
        var ratedFilms = await _films
            .Find(f => f.Rating != null)
            .ToListAsync();

        if (ratedFilms.Count > 0)
            stats.AverageRating = ratedFilms.Average(f => f.Rating!.Value);

        // Top subgenres
        var subgenreCounts = await _films.Aggregate()
            .Unwind<Film, FilmUnwound>(f => f.Subgenres)
            .Group(f => f.Subgenres, g => new SubgenreCount
            {
                Subgenre = g.Key,
                Count = g.Count()
            })
            .SortByDescending(s => s.Count)
            .Limit(10)
            .ToListAsync();

        stats.TopSubgenres = subgenreCounts;

        return stats;
    }

    private sealed class FilmUnwound
    {
        public string Id { get; set; } = null!;
        public string Subgenres { get; set; } = null!;
    }
}
