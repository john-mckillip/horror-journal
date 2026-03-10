using HorrorJournal.Api.Models;
using HorrorJournal.Api.Models.Requests;
using Microsoft.Extensions.Options;
using HorrorJournal.Api.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HorrorJournal.Api.Repositories;

public class FilmRepository : IFilmRepository
{
    private const string CountField = "count";
    private const string GroupStage = "$group";
    private const string IdField = "_id";
    private const string SumOp = "$sum";

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

    /// <summary>
    /// Uses a single $facet aggregation pipeline to compute all stats in one round-trip.
    /// Each facet runs an independent sub-pipeline against the films collection:
    ///   - totalCount: counts all documents
    ///   - countByStatus: groups by status field
    ///   - averageRating: filters to rated films, then computes $avg
    ///   - topSubgenres: $unwind subgenres array, group, sort desc, limit 5
    ///   - filmsPerYear: filters watched films with a watchedOn date, groups by year
    ///   - mostWatchedDirector: filters watched films, groups by director, sorts desc, limits 1
    /// </summary>
    public async Task<FilmStats> GetStatsAsync(CancellationToken ct = default)
    {
        var watchedFilter = new BsonDocument("status", FilmStatus.Watched.ToString());

        // $facet runs all sub-pipelines in a single aggregation pass
        var facetStage = new BsonDocument("$facet", new BsonDocument
        {
            { "totalCount", new BsonArray
                {
                    new BsonDocument("$count", CountField)
                }
            },
            { "countByStatus", new BsonArray
                {
                    new BsonDocument(GroupStage, new BsonDocument
                    {
                        { IdField, "$status" },
                        { CountField, new BsonDocument(SumOp, 1) }
                    })
                }
            },
            { "averageRating", new BsonArray
                {
                    new BsonDocument("$match", new BsonDocument("rating", new BsonDocument("$ne", BsonNull.Value))),
                    new BsonDocument(GroupStage, new BsonDocument
                    {
                        { IdField, BsonNull.Value },
                        { "avg", new BsonDocument("$avg", "$rating") }
                    })
                }
            },
            { "topSubgenres", new BsonArray
                {
                    new BsonDocument("$unwind", "$subgenres"),
                    new BsonDocument(GroupStage, new BsonDocument
                    {
                        { IdField, "$subgenres" },
                        { CountField, new BsonDocument(SumOp, 1) }
                    }),
                    new BsonDocument("$sort", new BsonDocument(CountField, -1)),
                    new BsonDocument("$limit", 5)
                }
            },
            { "filmsPerYear", new BsonArray
                {
                    new BsonDocument("$match", new BsonDocument
                    {
                        { "status", FilmStatus.Watched.ToString() },
                        { "watchedOn", new BsonDocument("$ne", BsonNull.Value) }
                    }),
                    new BsonDocument(GroupStage, new BsonDocument
                    {
                        { IdField, new BsonDocument("$year", "$watchedOn") },
                        { CountField, new BsonDocument(SumOp, 1) }
                    }),
                    new BsonDocument("$sort", new BsonDocument(IdField, 1))
                }
            },
            { "mostWatchedDirector", new BsonArray
                {
                    new BsonDocument("$match", watchedFilter),
                    new BsonDocument(GroupStage, new BsonDocument
                    {
                        { IdField, "$director" },
                        { CountField, new BsonDocument(SumOp, 1) }
                    }),
                    new BsonDocument("$sort", new BsonDocument(CountField, -1)),
                    new BsonDocument("$limit", 1)
                }
            }
        });

        var result = await _films
            .Aggregate()
            .AppendStage<BsonDocument>(facetStage)
            .FirstOrDefaultAsync(ct);

        return MapStatsFromBson(result);
    }
    /// <summary>
    /// Maps the raw BsonDocument result from the aggregation pipeline into a strongly-typed FilmStats object.
    /// </summary>
    /// <param name="result">The BsonDocument result from the aggregation pipeline.</param>
    /// <returns>A FilmStats object containing the mapped statistics.</returns>
    private static FilmStats MapStatsFromBson(BsonDocument result)
    {
        var stats = new FilmStats();

        if (result is null)
            return stats;

        // Total count
        var totalArr = result["totalCount"].AsBsonArray;
        stats.TotalCount = totalArr.Count > 0
            ? totalArr[0].AsBsonDocument[CountField].AsInt32
            : 0;

        // Count by status
        foreach (var doc in result["countByStatus"].AsBsonArray.Select(item => item.AsBsonDocument))
        {
            if (Enum.TryParse<FilmStatus>(doc[IdField].AsString, out var status))
                stats.CountByStatus[status] = doc[CountField].AsInt32;
        }

        // Average rating
        var avgArr = result["averageRating"].AsBsonArray;
        stats.AverageRating = avgArr.Count > 0
            ? Math.Round(avgArr[0].AsBsonDocument["avg"].ToDouble(), 1)
            : null;

        // Top 5 subgenres
        stats.TopSubgenres = [.. result["topSubgenres"].AsBsonArray
            .Select(item => item.AsBsonDocument)
            .Select(doc => new SubgenreCount
            {
                Subgenre = doc[IdField].AsString,
                Count = doc[CountField].AsInt32
            })];

        // Films watched per year
        stats.FilmsPerYear = [.. result["filmsPerYear"].AsBsonArray
            .Select(item => item.AsBsonDocument)
            .Select(doc => new YearCount
            {
                Year = doc[IdField].AsInt32,
                Count = doc[CountField].AsInt32
            })];

        // Most watched director
        var directorArr = result["mostWatchedDirector"].AsBsonArray;
        stats.MostWatchedDirector = directorArr.Count > 0
            ? directorArr[0].AsBsonDocument[IdField].AsString
            : null;

        return stats;
    }
}
