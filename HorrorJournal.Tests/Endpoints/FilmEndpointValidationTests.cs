using FluentAssertions;
using HorrorJournal.Api.Endpoints;
using HorrorJournal.Api.Models;
using HorrorJournal.Api.Models.Requests;

namespace HorrorJournal.Tests.Endpoints;

public class FilmEndpointValidationTests
{
    // ── CreateFilmRequest validation ──────────────────────────────────

    [Fact]
    public void ValidateCreateRequest_ValidRequest_ReturnsNoErrors()
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = 1982,
            Director = "John Carpenter",
            Status = FilmStatus.Watched
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateCreateRequest_NullTitle_ReturnsError()
    {
        var request = new CreateFilmRequest
        {
            Title = null,
            Year = 1982,
            Director = "John Carpenter"
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().ContainSingle().Which.Should().Be("Title is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateCreateRequest_EmptyOrWhitespaceTitle_ReturnsError(string title)
    {
        var request = new CreateFilmRequest
        {
            Title = title,
            Year = 1982,
            Director = "John Carpenter"
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().Contain("Title is required.");
    }

    [Fact]
    public void ValidateCreateRequest_NullDirector_ReturnsError()
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = 1982,
            Director = null
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().ContainSingle().Which.Should().Be("Director is required.");
    }

    [Fact]
    public void ValidateCreateRequest_MissingTitleAndDirector_ReturnsBothErrors()
    {
        var request = new CreateFilmRequest
        {
            Title = null,
            Year = 1982,
            Director = null
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().HaveCount(2);
        errors.Should().Contain("Title is required.");
        errors.Should().Contain("Director is required.");
    }

    [Theory]
    [InlineData(1887)]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidateCreateRequest_YearTooLow_ReturnsError(int year)
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = year,
            Director = "John Carpenter"
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().ContainSingle().Which.Should().StartWith("Year must be between 1888");
    }

    [Fact]
    public void ValidateCreateRequest_YearTooHigh_ReturnsError()
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = DateTime.UtcNow.Year + 6,
            Director = "John Carpenter"
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().ContainSingle().Which.Should().StartWith("Year must be between 1888");
    }

    [Theory]
    [InlineData(1888)]
    [InlineData(2024)]
    public void ValidateCreateRequest_YearAtBoundaries_ReturnsNoErrors(int year)
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = year,
            Director = "John Carpenter"
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(100)]
    public void ValidateCreateRequest_RatingOutOfRange_ReturnsError(int rating)
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = 1982,
            Director = "John Carpenter",
            Rating = rating
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().ContainSingle().Which.Should().Be("Rating must be between 1 and 10.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void ValidateCreateRequest_RatingInRange_ReturnsNoErrors(int rating)
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = 1982,
            Director = "John Carpenter",
            Rating = rating
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateCreateRequest_NullRating_ReturnsNoErrors()
    {
        var request = new CreateFilmRequest
        {
            Title = "The Thing",
            Year = 1982,
            Director = "John Carpenter",
            Rating = null
        };

        var errors = FilmEndpoints.ValidateCreateRequest(request);

        errors.Should().BeEmpty();
    }

    // ── FilmUpdate validation ─────────────────────────────────────────

    [Fact]
    public void ValidateUpdateRequest_AllNull_ReturnsNoErrors()
    {
        var update = new FilmUpdate();

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateUpdateRequest_ValidPartialUpdate_ReturnsNoErrors()
    {
        var update = new FilmUpdate
        {
            Title = "Hereditary",
            Rating = 9
        };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateUpdateRequest_EmptyTitle_ReturnsError(string title)
    {
        var update = new FilmUpdate { Title = title };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().ContainSingle().Which.Should().Be("Title cannot be empty.");
    }

    [Fact]
    public void ValidateUpdateRequest_NullTitle_IsAllowed()
    {
        var update = new FilmUpdate { Title = null };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateUpdateRequest_EmptyDirector_ReturnsError(string director)
    {
        var update = new FilmUpdate { Director = director };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().ContainSingle().Which.Should().Be("Director cannot be empty.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void ValidateUpdateRequest_RatingOutOfRange_ReturnsError(int rating)
    {
        var update = new FilmUpdate { Rating = rating };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().ContainSingle().Which.Should().Be("Rating must be between 1 and 10.");
    }

    [Fact]
    public void ValidateUpdateRequest_YearTooLow_ReturnsError()
    {
        var update = new FilmUpdate { Year = 1887 };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().ContainSingle().Which.Should().StartWith("Year must be between 1888");
    }

    [Fact]
    public void ValidateUpdateRequest_YearTooHigh_ReturnsError()
    {
        var update = new FilmUpdate { Year = DateTime.UtcNow.Year + 6 };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().ContainSingle().Which.Should().StartWith("Year must be between 1888");
    }

    [Fact]
    public void ValidateUpdateRequest_MultipleErrors_ReturnsAll()
    {
        var update = new FilmUpdate
        {
            Title = "",
            Director = "  ",
            Year = 1800,
            Rating = 0
        };

        var errors = FilmEndpoints.ValidateUpdateRequest(update);

        errors.Should().HaveCount(4);
    }
}
