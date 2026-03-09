namespace HorrorJournal.Api.Models.Requests;
/// <summary>
/// Represents the data required to create a new horror film entry in the journal. 
/// This class includes properties for all the necessary details of a horror film, 
/// such as Title, Year, Director, Subgenres, Status, Rating, WatchedOn, and Notes. 
/// When a user submits a request to add a new film to their journal, they will provide 
/// this information in the request body, which will then be used to create a new Film 
/// object and store it in the database. By using this class for create operations, 
/// users can ensure that they provide all the relevant information needed to 
/// accurately represent their horror film entries in the journal.
/// </summary>
public class CreateFilmRequest
{
    /// <summary>
    /// The title of the horror film. It will be used to identify 
    /// and display the film in the user's collection.
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// The release year of the horror film.
    /// </summary>
    public int Year { get; set; }
    /// <summary>
    /// The director of the horror film.
    /// </summary>
    public string? Director { get; set; }
    /// <summary>
    /// A list of subgenres that the horror film belongs to (e.g., slasher, supernatural, psychological).
    /// </summary>
    public List<string> Subgenres { get; set; } = [];
    /// <summary>
    /// The status of the film in the journal, indicating whether it has been watched, is planned to be watched, or is currently being watched.
    /// </summary>
    public FilmStatus Status { get; set; }
    /// <summary>
    /// The rating of the horror film, typically on a scale from 1 to 10.
    /// </summary>
    public int? Rating { get; set; }
    /// <summary>
    /// The date when the horror film was watched by the user. This field is optional and can be left 
    /// null if the user has not yet watched the film or does not wish to provide this information.
    /// </summary>
    public DateTime? WatchedOn { get; set; }
    /// <summary>
    /// Additional notes or comments about the horror film. This field is optional and can be used by the 
    /// user to provide any extra information, thoughts, or impressions about the film that they wish to record in their journal.
    /// </summary>
    public string? Notes { get; set; }
}
