using FluentAssertions;
using HorrorJournal.Api.Models;
using HorrorJournal.Api.Models.Requests;
using HorrorJournal.Api.Repositories;
using HorrorJournal.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;

namespace HorrorJournal.Tests.Repositories;

public class FilmRepositoryFilterTests
{
    private readonly Mock<IMongoCollection<Film>> _mockCollection;
    private readonly FilmRepository _repository;

    public FilmRepositoryFilterTests()
    {
        _mockCollection = new Mock<IMongoCollection<Film>>();

        var mockDatabase = new Mock<IMongoDatabase>();
        mockDatabase
            .Setup(db => db.GetCollection<Film>("films", null))
            .Returns(_mockCollection.Object);

        var mockClient = new Mock<IMongoClient>();
        mockClient
            .Setup(c => c.GetDatabase("horror_journal", null))
            .Returns(mockDatabase.Object);

        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "horror_journal"
        });

        _repository = new FilmRepository(mockClient.Object, settings);
    }

    /// <summary>
    /// Helper: captures the FilterDefinition passed to FindAsync and renders it to BsonDocument.
    /// </summary>
    private BsonDocument CaptureFilter(Action<FilmFilter> act)
    {
        FilterDefinition<Film>? captured = null;

        var mockCursor = new Mock<IAsyncCursor<Film>>();
        mockCursor.Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Film>>(),
                It.IsAny<FindOptions<Film, Film>>(),
                It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Film>, FindOptions<Film, Film>, CancellationToken>(
                (filter, _, _) => captured = filter)
            .ReturnsAsync(mockCursor.Object);

        var filter = new FilmFilter();
        act(filter);

        _repository.GetAllAsync(filter).GetAwaiter().GetResult();

        captured.Should().NotBeNull("the filter should have been captured");

        var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Film>();
        return captured!.Render(new RenderArgs<Film>(serializer, BsonSerializer.SerializerRegistry));
    }

    [Fact]
    public void GetAllAsync_NoFilters_ProducesEmptyFilter()
    {
        var bson = CaptureFilter(_ => { });

        bson.ElementCount.Should().Be(0);
    }

    [Fact]
    public void GetAllAsync_StatusFilter_ProducesEqOnStatus()
    {
        var bson = CaptureFilter(f => f.Status = FilmStatus.Watched);

        bson.Contains("status").Should().BeTrue();
        bson["status"].AsString.Should().Be("Watched");
    }

    [Fact]
    public void GetAllAsync_SubgenreFilter_ProducesAnyEqOnSubgenres()
    {
        var bson = CaptureFilter(f => f.Subgenre = "Slasher");

        bson.Contains("subgenres").Should().BeTrue();
        bson["subgenres"].AsString.Should().Be("Slasher");
    }

    [Fact]
    public void GetAllAsync_MinRatingFilter_ProducesGteOnRating()
    {
        var bson = CaptureFilter(f => f.MinRating = 7);

        bson.Contains("rating").Should().BeTrue();
        bson["rating"].AsBsonDocument.Contains("$gte").Should().BeTrue();
        bson["rating"]["$gte"].AsInt32.Should().Be(7);
    }

    [Fact]
    public void GetAllAsync_AllFilters_CombinesAllConditions()
    {
        var bson = CaptureFilter(f =>
        {
            f.Status = FilmStatus.Wishlist;
            f.Subgenre = "Supernatural";
            f.MinRating = 5;
        });

        // The driver merges And filters into a single document when keys don't conflict
        bson.Contains("status").Should().BeTrue();
        bson.Contains("subgenres").Should().BeTrue();
        bson.Contains("rating").Should().BeTrue();
        bson["status"].AsString.Should().Be("Wishlist");
        bson["subgenres"].AsString.Should().Be("Supernatural");
        bson["rating"]["$gte"].AsInt32.Should().Be(5);
    }

    [Fact]
    public void GetAllAsync_NullSubgenre_DoesNotFilterOnSubgenre()
    {
        var bson = CaptureFilter(f =>
        {
            f.Status = FilmStatus.Abandoned;
            f.Subgenre = null;
        });

        bson.Contains("status").Should().BeTrue();
        bson.Contains("subgenres").Should().BeFalse();
    }

    [Fact]
    public void GetAllAsync_EmptySubgenre_DoesNotFilterOnSubgenre()
    {
        var bson = CaptureFilter(f => f.Subgenre = "   ");

        bson.ElementCount.Should().Be(0, "whitespace-only subgenre should be ignored");
    }
}
