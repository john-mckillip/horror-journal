namespace HorrorJournal.Api.Models.Requests;
/// <summary>
/// Represents the filtering criteria for retrieving horror films from the journal. This class includes optional properties for filtering films based on their status (watched, wishlist, abandoned), subgenre, and minimum rating. By providing these filtering parameters, users can customize their queries to retrieve specific subsets of their horror film collection that match their preferences and organizational needs.
/// </summary>
public class FilmFilter
{
    /// <summary>
    /// The status of the horror films to filter by, indicating whether the films have been watched, are on the user's wishlist, or have been abandoned. This property is optional and can be left null if no filtering by status is desired. When specified, only films that match the given status will be included in the results.
    /// </summary>
    public FilmStatus? Status { get; set; }
    /// <summary>
    /// The subgenre of the horror films to filter by (e.g., slasher, supernatural, psychological). This property is optional and can be left null if no filtering by subgenre is desired. When specified, only films that belong to the given subgenre will be included in the results
    /// Note: The value for this property should ideally match one of the predefined subgenre tags defined in the SubgenreTag class to ensure consistency when categorizing films by their subgenres.
    /// </summary>
    public string? Subgenre { get; set; }
    /// <summary>
    /// The minimum rating of the horror films to filter by. This property is optional and can be left null if no filtering by rating is desired. When specified, only films with a rating equal to or greater than the given value will be included in the results.
    /// </summary>
    public int? MinRating { get; set; }
}
