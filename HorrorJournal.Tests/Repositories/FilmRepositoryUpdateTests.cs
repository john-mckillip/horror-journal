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

public class FilmRepositoryUpdateTests
{
    private readonly Mock<IMongoCollection<Film>> _mockCollection;
    private readonly FilmRepository _repository;

    public FilmRepositoryUpdateTests()
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
    /// Helper: captures the UpdateDefinition passed to UpdateOneAsync and renders the $set document.
    /// </summary>
    private BsonDocument CaptureUpdateSet(FilmUpdate update)
    {
        UpdateDefinition<Film>? captured = null;

        var mockResult = new Mock<UpdateResult>();
        mockResult.Setup(r => r.MatchedCount).Returns(1);

        _mockCollection
            .Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<Film>>(),
                It.IsAny<UpdateDefinition<Film>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Film>, UpdateDefinition<Film>, UpdateOptions, CancellationToken>(
                (_, upd, _, _) => captured = upd)
            .ReturnsAsync(mockResult.Object);

        _repository.UpdateAsync("507f1f77bcf86cd799439011", update).GetAwaiter().GetResult();

        captured.Should().NotBeNull("the update definition should have been captured");

        var serializer = BsonSerializer.SerializerRegistry.GetSerializer<Film>();
        var rendered = captured!.Render(new RenderArgs<Film>(serializer, BsonSerializer.SerializerRegistry))
            .AsBsonDocument;

        rendered.Contains("$set").Should().BeTrue();
        return rendered["$set"].AsBsonDocument;
    }

    [Fact]
    public void UpdateAsync_EmptyUpdate_AlwaysSetsUpdatedAt()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate());

        setDoc.Contains("updatedAt").Should().BeTrue();
        setDoc.ElementCount.Should().Be(1, "only updatedAt should be set when no fields are provided");
    }

    [Fact]
    public void UpdateAsync_TitleProvided_SetsTitleAndUpdatedAt()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate { Title = "Midsommar" });

        setDoc.Contains("title").Should().BeTrue();
        setDoc["title"].AsString.Should().Be("Midsommar");
        setDoc.Contains("updatedAt").Should().BeTrue();
    }

    [Fact]
    public void UpdateAsync_YearProvided_SetsYear()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate { Year = 2019 });

        setDoc.Contains("year").Should().BeTrue();
        setDoc["year"].AsInt32.Should().Be(2019);
    }

    [Fact]
    public void UpdateAsync_DirectorProvided_SetsDirector()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate { Director = "Ari Aster" });

        setDoc.Contains("director").Should().BeTrue();
        setDoc["director"].AsString.Should().Be("Ari Aster");
    }

    [Fact]
    public void UpdateAsync_SubgenresProvided_SetsSubgenres()
    {
        var subgenres = new List<string> { "Folk Horror", "Psychological" };
        var setDoc = CaptureUpdateSet(new FilmUpdate { Subgenres = subgenres });

        setDoc.Contains("subgenres").Should().BeTrue();
        var arr = setDoc["subgenres"].AsBsonArray;
        arr.Should().HaveCount(2);
        arr[0].AsString.Should().Be("Folk Horror");
        arr[1].AsString.Should().Be("Psychological");
    }

    [Fact]
    public void UpdateAsync_StatusProvided_SetsStatusAsString()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate { Status = FilmStatus.Abandoned });

        setDoc.Contains("status").Should().BeTrue();
        setDoc["status"].AsString.Should().Be("Abandoned");
    }

    [Fact]
    public void UpdateAsync_RatingProvided_SetsRating()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate { Rating = 8 });

        setDoc.Contains("rating").Should().BeTrue();
        setDoc["rating"].AsInt32.Should().Be(8);
    }

    [Fact]
    public void UpdateAsync_WatchedOnProvided_SetsWatchedOn()
    {
        var date = new DateTime(2024, 10, 31, 0, 0, 0, DateTimeKind.Utc);
        var setDoc = CaptureUpdateSet(new FilmUpdate { WatchedOn = date });

        setDoc.Contains("watchedOn").Should().BeTrue();
    }

    [Fact]
    public void UpdateAsync_NotesProvided_SetsNotes()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate { Notes = "Unsettling atmosphere" });

        setDoc.Contains("notes").Should().BeTrue();
        setDoc["notes"].AsString.Should().Be("Unsettling atmosphere");
    }

    [Fact]
    public void UpdateAsync_MultipleFields_OnlySetsProvidedFieldsPlusUpdatedAt()
    {
        var setDoc = CaptureUpdateSet(new FilmUpdate
        {
            Title = "Hereditary",
            Rating = 10
        });

        setDoc.Contains("title").Should().BeTrue();
        setDoc.Contains("rating").Should().BeTrue();
        setDoc.Contains("updatedAt").Should().BeTrue();
        setDoc.ElementCount.Should().Be(3, "only title, rating, and updatedAt should be set");

        // Fields NOT provided should be absent
        setDoc.Contains("year").Should().BeFalse();
        setDoc.Contains("director").Should().BeFalse();
        setDoc.Contains("subgenres").Should().BeFalse();
        setDoc.Contains("status").Should().BeFalse();
        setDoc.Contains("watchedOn").Should().BeFalse();
        setDoc.Contains("notes").Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_MatchedCountZero_ReturnsFalse()
    {
        var mockResult = new Mock<UpdateResult>();
        mockResult.Setup(r => r.MatchedCount).Returns(0);

        _mockCollection
            .Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<Film>>(),
                It.IsAny<UpdateDefinition<Film>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var result = await _repository.UpdateAsync("nonexistent", new FilmUpdate { Title = "X" });

        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_MatchedCountPositive_ReturnsTrue()
    {
        var mockResult = new Mock<UpdateResult>();
        mockResult.Setup(r => r.MatchedCount).Returns(1);

        _mockCollection
            .Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<Film>>(),
                It.IsAny<UpdateDefinition<Film>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        var result = await _repository.UpdateAsync("507f1f77bcf86cd799439011", new FilmUpdate { Title = "X" });

        result.Should().BeTrue();
    }
}
