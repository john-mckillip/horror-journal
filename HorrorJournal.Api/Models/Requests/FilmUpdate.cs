namespace HorrorJournal.Api.Models.Requests;
/// <summary>
/// Represents the data required to update an existing horror film entry in the journal. This class includes optional properties for each field that can be updated, allowing users to modify only the specific details they wish to change without needing to provide all information for the film. The properties include Title, Year, Director, Subgenres, Status, Rating, WatchedOn, and Notes. By using this class in update operations, users can efficiently manage their horror film entries while maintaining flexibility in how they update their information.
/// </summary>
public class FilmUpdate
{
    /// <summary>
    /// The title of the horror film. This property is optional and can be left null if the title is not being updated. When provided, it will replace the existing title of the film entry in the journal.
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// The release year of the horror film. This property is optional and can be left null if the year is not being updated. When provided, it will replace the existing release year of the film entry in the journal.
    /// </summary>
    public int? Year { get; set; }
    /// <summary>
    /// The director of the horror film. This property is optional and can be left null if the director is not being updated. When provided, it will replace the existing director of the film entry in the journal.
    /// </summary>
    public string? Director { get; set; }
    /// <summary>
    /// The list of subgenres for the horror film. This property is optional and can be left null if the subgenres are not being updated. When provided, it will replace the existing subgenres of the film entry in the journal.
    /// </summary>
    public List<string>? Subgenres { get; set; }
    /// <summary>
    /// The status of the film in the journal, indicating whether it has been watched, is planned to be watched, or is currently being watched. This property is optional and can be left null if the status is not being updated. When provided, it will replace the existing status of the film entry in the journal.
    /// </summary>
    public FilmStatus? Status { get; set; }
    /// <summary>
    /// The rating given to the horror film by the user, typically on a scale (e.g., 1 to 10). This property is optional and can be left null if the rating is not being updated. When provided, it will replace the existing rating of the film entry in the journal.
    /// </summary>
    public int? Rating { get; set; }
    /// <summary>
    /// The date when the horror film was watched by the user. This property is optional and can be left null if the watched date is not being updated. When provided, it will replace the existing watched date of the film entry in the journal.
    /// </summary>
    public DateTime? WatchedOn { get; set; }
    /// <summary>
    /// Additional notes or comments about the horror film. This property is optional and can be left null if no notes are being updated. When provided, it will replace the existing notes of the film entry in the journal.
    /// </summary>
    public string? Notes { get; set; }
}
