using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HorrorJournal.Api.Models;
/// <summary>
/// Represents a horror film in the journal, including details such as title, year, director, subgenres, status, rating, watched date, notes, and timestamps for creation and updates.
/// </summary>
public class Film
{
    /// <summary>
    /// The unique identifier for the film, represented as a MongoDB ObjectId. This is automatically generated when a new film is added to the database.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    /// <summary>
    /// The title of the horror film. This is a required field and must be provided when creating or updating a film entry.
    /// </summary>
    [BsonElement("title")]
    public required string Title { get; set; }
    /// <summary>
    /// The release year of the horror film. This is a required field and must be provided when creating or updating a film entry.
    /// </summary>
    [BsonElement("year")]
    public int Year { get; set; }
    /// <summary>
    /// The director of the horror film. This is a required field and must be provided when creating or updating a film entry.
    /// </summary>
    [BsonElement("director")]
    public required string Director { get; set; }
    /// <summary>
    /// A list of subgenres that the horror film belongs to (e.g., slasher, supernatural, psychological). This field is optional and can be left empty if no subgenres are specified.
    /// </summary>
    [BsonElement("subgenres")]
    public List<string> Subgenres { get; set; } = [];
    /// <summary>
    /// The status of the film in the journal, indicating whether it has been watched, is planned to be watched, or is currently being watched. This is a required field and must be provided when creating or updating a film entry.
    /// </summary>
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public FilmStatus Status { get; set; }
    /// <summary>
    /// The rating given to the horror film by the user, typically on a scale (e.g., 1 to 10). This field is optional and can be left null if the user has not provided a rating for the film.
    /// </summary>
    [BsonElement("rating")]
    public int? Rating { get; set; }
    /// <summary>
    /// The date when the horror film was watched by the user. This field is optional and can be left null if the user has not yet watched the film or does not wish to provide this information.
    /// </summary>
    [BsonElement("watchedOn")]
    public DateTime? WatchedOn { get; set; }
    /// <summary>
    /// Additional notes or comments about the horror film. This field is optional and can be left null if the user does not have any specific notes to add about the film.
    /// </summary>
    [BsonElement("notes")]
    public string? Notes { get; set; }
    /// <summary>
    /// The date and time when the film entry was created in the journal. This is automatically set to the current date and time when a new film is added to the database and should not be modified by the user.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// The date and time when the film entry was last updated in the journal. This is automatically set to the current date and time whenever the film entry is modified.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
