using HorrorJournal.Api.Models;
using HorrorJournal.Api.Models.Requests;
using HorrorJournal.Api.Repositories;

namespace HorrorJournal.Api.Endpoints;
/// <summary>
/// This class defines the API endpoints for managing horror film entries in the journal.
/// </summary>
public static class FilmEndpoints
{
    /// <summary>
    /// Maps the film-related endpoints to the WebApplication instance. This method sets up the routes for CRUD operations on horror film entries, as well as additional endpoints for searching and retrieving statistics about the films in the journal. Each endpoint is associated with a specific HTTP method (GET, POST, PUT, DELETE) and a corresponding handler method that processes the request and interacts with the film repository to perform the necessary operations on the database. By calling this method in the Program.cs file, we can ensure that all film-related API routes are properly configured and ready to handle incoming requests from clients.
    /// </summary>
    /// <param name="app">The WebApplication instance to which the film endpoints will be mapped.</param>
    public static void MapFilmEndpoints(this WebApplication app)
    {
        var films = app.MapGroup("/films");

        films.MapGet("/", GetAllAsync);
        films.MapGet("/search", SearchAsync);
        films.MapGet("/stats", GetStatsAsync);
        films.MapGet("/{id}", GetByIdAsync);
        films.MapPost("/", CreateAsync);
        films.MapPut("/{id}", UpdateAsync);
        films.MapDelete("/{id}", DeleteAsync);
    }
    /// <summary>
    /// Handles the GET request to retrieve all horror film entries from the journal, with optional filtering based on status, subgenre, and minimum rating. This method interacts with the film repository to fetch the relevant data from the database and returns it in the response. If any of the query parameters are invalid (e.g., minRating is out of range), it returns a Bad Request response with an appropriate error message. Otherwise, it returns an OK response containing the list of films that match the specified criteria.
    /// </summary>
    /// <param name="repo">The film repository used to access horror film data.</param>
    /// <param name="status">Optional filter for the status of the films.</param>
    /// <param name="subgenre">Optional filter for the subgenre of the films.</param>
    /// <param name="minRating">Optional filter for the minimum rating of the films.</param>
    /// <returns>An IResult containing the filtered list of films or an error message.</returns>
    private static async Task<IResult> GetAllAsync(
        IFilmRepository repo,
        FilmStatus? status,
        string? subgenre,
        int? minRating)
    {
        if (minRating.HasValue && minRating is < 1 or > 10)
            return Results.BadRequest("minRating must be between 1 and 10.");

        var filter = new FilmFilter
        {
            Status = status,
            Subgenre = subgenre,
            MinRating = minRating
        };

        return Results.Ok(await repo.GetAllAsync(filter));
    }
    /// <summary>
    /// Handles the GET request to search for horror film entries in the journal based on a query string. This method interacts with the film repository to perform a search operation that looks for matches in the title, director, and notes fields of the film entries. If the query parameter 'q' is missing or empty, it returns a Bad Request response indicating that the query parameter is required. Otherwise, it returns an OK response containing the list of films that match the search criteria. This endpoint allows users to quickly find specific films in their journal by providing relevant keywords or phrases related to the film's details.
    /// </summary>
    /// <param name="repo">The film repository used to access horror film data.</param>
    /// <param name="q">The query string used to search for films.</param>
    /// <returns>An IResult containing the list of films that match the search criteria or an error message.</returns>
    private static async Task<IResult> SearchAsync(IFilmRepository repo, string? q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Results.BadRequest("Query parameter 'q' is required.");

        return Results.Ok(await repo.SearchAsync(q));
    }
    /// <summary>
    /// Handles the GET request to retrieve statistics about the horror film entries in the journal. This method interacts with the film repository to gather various statistics, such as the total number of films, average rating, distribution of films by status, and other relevant insights. The collected statistics are then returned in an OK response. This endpoint provides users with a quick overview of their horror film collection and helps them understand trends and patterns in their viewing habits.
    /// </summary>
    /// <param name="repo">The film repository used to access horror film data.</param>
    /// <returns>An IResult containing the statistics of the horror film entries.</returns>
    private static async Task<IResult> GetStatsAsync(IFilmRepository repo)
    {
        return Results.Ok(await repo.GetStatsAsync());
    }
    /// <summary>
    /// Handles the GET request to retrieve a specific horror film entry from the journal based on its unique identifier (id). This method interacts with the film repository to fetch the film data corresponding to the provided id. If a film with the specified id is found, it returns an OK response containing the film details. If no film is found with the given id, it returns a Not Found response. This endpoint allows users to access detailed information about a specific film in their journal by providing its unique identifier.
    /// </summary>
    /// <param name="repo">The film repository used to access horror film data.</param>
    /// <param name="id">The unique identifier of the horror film entry.</param>
    /// <returns>An IResult containing the film details or a Not Found response.</returns>
    private static async Task<IResult> GetByIdAsync(IFilmRepository repo, string id)
    {
        var film = await repo.GetByIdAsync(id);
        return film is not null ? Results.Ok(film) : Results.NotFound();
    }
    /// <summary>
    /// Handles the POST request to create a new horror film entry in the journal. This method receives a CreateFilmRequest object containing the details of the film to be created. It first validates the request data to ensure that all required fields are provided and that the values are within acceptable ranges (e.g., year, rating). If any validation errors are found, it returns a Bad Request response with a list of error messages. If the request is valid, it creates a new Film object based on the provided data and inserts it into the database using the film repository. Finally, it returns a Created response with the location of the newly created film entry and its details in the response body. This endpoint allows users to add new films to their journal by providing the necessary information about each film they want to track.
    /// </summary>
    /// <param name="repo">The film repository used to access horror film data.</param>
    /// <param name="request">The request object containing the details of the film to be created.</param>
    /// <returns>An IResult containing the details of the newly created film entry.</returns>
    private static async Task<IResult> CreateAsync(IFilmRepository repo, CreateFilmRequest request)
    {
        var errors = ValidateCreateRequest(request);
        if (errors.Count > 0)
            return Results.BadRequest(new { errors });

        var film = new Film
        {
            Title = request.Title!,
            Year = request.Year,
            Director = request.Director!,
            Subgenres = request.Subgenres,
            Status = request.Status,
            Rating = request.Rating,
            WatchedOn = request.WatchedOn,
            Notes = request.Notes
        };

        await repo.InsertAsync(film);
        return Results.Created($"/films/{film.Id}", film);
    }
    /// <summary>
    /// Handles the PUT request to update an existing horror film entry in the journal based on its unique identifier (id). This method receives a FilmUpdate object containing the fields to be updated for the specified film. It first validates the update request to ensure that any provided values are within acceptable ranges (e.g., year, rating) and that string fields are not empty if they are included in the update. If any validation errors are found, it returns a Bad Request response with a list of error messages. If the request is valid, it attempts to update the film entry in the database using the film repository. If a film with the specified id is found and successfully updated, it returns a No Content response. If no film is found with the given id, it returns a Not Found response. This endpoint allows users to modify existing film entries in their journal by providing only the fields they wish to update, while leaving other fields unchanged.
    /// </summary>
    /// <param name="repo">The film repository used to access horror film data.</param>
    /// <param name="id">The unique identifier of the film to be updated.</param>
    /// <param name="update">The object containing the fields to be updated for the specified film.</param>
    /// <returns>An IResult indicating the outcome of the update operation.</returns>
    private static async Task<IResult> UpdateAsync(IFilmRepository repo, string id, FilmUpdate update)
    {
        var errors = ValidateUpdateRequest(update);
        if (errors.Count > 0)
            return Results.BadRequest(new { errors });

        var found = await repo.UpdateAsync(id, update);
        return found ? Results.NoContent() : Results.NotFound();
    }
    /// <summary>
    /// Handles the DELETE request to remove a specific horror film entry from the journal based on its unique identifier (id). This method interacts with the film repository to delete the film data corresponding to the provided id. If a film with the specified id is found and successfully deleted, it returns a No Content response. If no film is found with the given id, it returns a Not Found response. This endpoint allows users to remove films from their journal by providing the unique identifier of the film they wish to delete.
    /// </summary>
    /// <param name="repo">The film repository used to access horror film data.</param>
    /// <param name="id">The unique identifier of the film to be deleted.</param>
    /// <returns>An IResult indicating the outcome of the delete operation.</returns>
    private static async Task<IResult> DeleteAsync(IFilmRepository repo, string id)
    {
        var found = await repo.DeleteAsync(id);
        return found ? Results.NoContent() : Results.NotFound();
    }
    /// <summary>
    /// Validates the CreateFilmRequest object to ensure that all required fields are provided and that the values are within acceptable ranges. This method checks for the presence of mandatory fields such as Title and Director, and validates that the Year is within a reasonable range (e.g., not before the invention of film and not too far in the future). It also checks that if a Rating is provided, it falls within the typical 1 to 10 scale. If any validation errors are found, it returns a list of error messages that can be included in a Bad Request response to inform the client about the issues with their request data.
    /// </summary>
    /// <param name="request">The CreateFilmRequest object containing the data to be validated.</param>
    /// <returns>A list of error messages indicating any validation issues found in the request.</returns>
    private static List<string> ValidateCreateRequest(CreateFilmRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("Title is required.");

        if (string.IsNullOrWhiteSpace(request.Director))
            errors.Add("Director is required.");

        if (request.Year < 1888 || request.Year > DateTime.UtcNow.Year + 5)
            errors.Add($"Year must be between 1888 and {DateTime.UtcNow.Year + 5}.");

        if (request.Rating.HasValue && request.Rating is < 1 or > 10)
            errors.Add("Rating must be between 1 and 10.");

        return errors;
    }
    /// <summary>
    /// Validates the FilmUpdate object to ensure that any provided values are within acceptable ranges and that string fields are not empty if they are included in the update. This method checks that if the Title or Director fields are included in the update, they are not empty or whitespace. It also validates that if the Year is provided, it falls within a reasonable range (e.g., not before the invention of film and not too far in the future). Additionally, it checks that if a Rating is provided, it falls within the typical 1 to 10 scale. If any validation errors are found, it returns a list of error messages that can be included in a Bad Request response to inform the client about the issues with their update request data.
    /// </summary>
    /// <param name="update">The FilmUpdate object containing the data to be validated.</param>
    /// <returns>A list of error messages indicating any validation issues found in the update request.</returns>
    private static List<string> ValidateUpdateRequest(FilmUpdate update)
    {
        var errors = new List<string>();

        if (update.Title is not null && string.IsNullOrWhiteSpace(update.Title))
            errors.Add("Title cannot be empty.");

        if (update.Director is not null && string.IsNullOrWhiteSpace(update.Director))
            errors.Add("Director cannot be empty.");

        if (update.Year.HasValue && (update.Year < 1888 || update.Year > DateTime.UtcNow.Year + 5))
            errors.Add($"Year must be between 1888 and {DateTime.UtcNow.Year + 5}.");

        if (update.Rating.HasValue && update.Rating is < 1 or > 10)
            errors.Add("Rating must be between 1 and 10.");

        return errors;
    }
}
