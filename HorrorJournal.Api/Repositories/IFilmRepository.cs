using HorrorJournal.Api.Models;
using HorrorJournal.Api.Models.Requests;

namespace HorrorJournal.Api.Repositories;
/// <summary>
/// Defines an interface for a repository that manages horror film entries in the journal. This interface includes methods for retrieving all films with optional filtering, getting a film by its unique identifier, searching for films based on text queries, inserting new film entries, updating existing film entries, deleting film entries, and retrieving statistics about the films in the journal. Implementing this interface allows for consistent data access and manipulation of horror film entries across the application.
/// </summary>
public interface IFilmRepository
{
    /// <summary>
    /// Retrieves a list of all horror films in the journal, with optional filtering based on criteria such as status, subgenre, or release year. The method accepts a FilmFilter object that contains the filtering parameters and returns a list of Film objects that match the specified criteria. This allows users to easily access and view their horror film entries based on their preferences and organizational needs.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<List<Film>> GetAllAsync(FilmFilter filter);
    /// <summary>
    /// Retrieves a specific horror film from the journal based on its unique identifier (ID). The method accepts a string parameter representing the film's ID and returns a Film object if a matching entry is found, or null if no such entry exists. This allows users to access detailed information about a specific horror film in their journal by referencing its unique ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Film?> GetByIdAsync(string id);
    /// <summary>
    /// Searches for horror films in the journal based on a text query. The method accepts a string parameter representing the search text and returns a list of Film objects that match the query in relevant fields such as title, director, or notes. This allows users to quickly find specific horror films in their journal by searching for keywords or phrases associated with the films they have entered.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    Task<List<Film>> SearchAsync(string text);
    /// <summary>
    /// Inserts a new horror film entry into the journal. The method accepts a Film object containing the details of the film to be added and returns a Task representing the asynchronous operation. This allows users to add new horror films to their journal, including all relevant information such as title, year, director, subgenres, status, rating, watched date, and notes. The repository implementation will handle the actual insertion of the film entry into the underlying data store (e.g., MongoDB).
    /// </summary>
    /// <param name="film"></param>
    /// <returns></returns>
    Task InsertAsync(Film film);
    /// <summary>
    /// Updates an existing horror film entry in the journal based on its unique identifier (ID). The method accepts a string parameter representing the film's ID and a FilmUpdate object containing the updated details of the film. It returns a boolean value indicating whether the update operation was successful (true) or if no matching entry was found to update (false). This allows users to modify the details of their existing horror film entries in the journal, ensuring that their information remains accurate and up-to-date. The repository implementation will handle the actual update of the film entry in the underlying data store (e.g., MongoDB).
    /// </summary>
    /// <param name="id"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(string id, FilmUpdate update);
    /// <summary>
    /// Deletes a horror film entry from the journal based on its unique identifier (ID). The method accepts a string parameter representing the film's ID and returns a boolean value indicating whether the delete operation was successful (true) or if no matching entry was found to delete (false). This allows users to remove horror film entries from their journal when they are no longer relevant or needed. The repository implementation will handle the actual deletion of the film entry from the underlying data store (e.g., MongoDB).
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(string id);
    /// <summary>
    /// Retrieves statistics about the horror films in the journal, such as the total number of films, the number of films by status (watched, wishlist, abandoned), the average rating, and other relevant metrics. The method returns a FilmStats object containing these statistics, allowing users to gain insights into their horror film collection and track their viewing habits over time. The repository implementation will handle the calculation and retrieval of these statistics from the underlying data store (e.g., MongoDB).
    /// </summary>
    /// <returns></returns>
    Task<FilmStats> GetStatsAsync(CancellationToken ct = default);
}
