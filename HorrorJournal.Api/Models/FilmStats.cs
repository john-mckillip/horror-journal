namespace HorrorJournal.Api.Models;
/// <summary>
/// Represents statistical information about the horror films in the journal, including counts of films by status, average rating, and top subgenres. This class is used to provide insights and summaries about the user's horror film collection, allowing them to see trends and patterns in their viewing habits and preferences. The CountByStatus property provides a breakdown of how many films fall into each status category (watched, wishlist, abandoned), while the AverageRating property gives an overall average rating for all films in the journal. The TopSubgenres property lists the most common subgenres among the films, along with their respective counts, helping users identify which types of horror films they tend to watch or prefer.
/// </summary>
public class FilmStats
{
    /// <summary>
    /// A dictionary that counts the number of films in the journal for each FilmStatus (Watched, Wishlist, Abandoned). This allows users to see how many films they have watched, how many are on their wishlist, and how many they have abandoned, providing insights into their viewing habits and preferences.
    /// The keys of the dictionary are of type FilmStatus, and the values are integers representing the count of films for each status category.
    /// For example, if a user has watched 10 films, has 5 films on their wishlist, and has abandoned 2 films, the CountByStatus dictionary would contain the following entries:
    /// {
    ///     FilmStatus.Watched: 10,
    ///     FilmStatus.Wishlist: 5,
    ///     FilmStatus.Abandoned: 2
    /// }
    /// This information can be used to generate summaries and visualizations of the user's horror film collection based on their interaction with the films.
    /// </summary>
    public Dictionary<FilmStatus, int> CountByStatus { get; set; } = [];
    /// <summary>
    /// The average rating of all horror films in the journal. This is calculated by taking the sum of all film ratings and dividing it by the number of films that have a rating. The AverageRating property provides an overall measure of the user's satisfaction with their horror film collection, allowing them to see how they have rated their films on average. If there are no films with ratings, this property can be null to indicate that an average rating cannot be calculated.
    /// For example, if a user has rated 5 films with ratings of 8, 7, 9, 6, and 10, the AverageRating would be calculated as follows:
    /// </summary>
    public double? AverageRating { get; set; }
    /// <summary>
    /// A list of the top subgenres among the horror films in the journal, along with their respective counts. This property provides insights into which subgenres are most common in the user's horror film collection, allowing them to identify trends and preferences in the types of horror films they watch. Each entry in the TopSubgenres list is an instance of the SubgenreCount class, which contains a Subgenre property (the name of the subgenre) and a Count property (the number of films that belong to that subgenre). This information can be used to generate summaries and visualizations of the user's horror film collection based on subgenre preferences.
    /// For example, if a user has 10 films in the "Slasher" subgenre, 7 films in the "Supernatural" subgenre, and 5 films in the "Psychological" subgenre, the TopSubgenres list would contain the following entries:
    /// </summary>
    public List<SubgenreCount> TopSubgenres { get; set; } = [];
}
/// <summary>
/// Represents a count of horror films for a specific subgenre in the journal. This class is used as part of the FilmStats to provide insights into the distribution of subgenres among the user's horror film collection. The Subgenre property contains the name of the subgenre (e.g., "Slasher", "Supernatural"), while the Count property indicates how many films in the journal belong to that subgenre. This information can help users identify which types of horror films they tend to watch or prefer, and can be used to generate summaries and visualizations based on subgenre preferences.
/// </summary>
public class SubgenreCount
{
    /// <summary>
    /// The name of the horror film subgenre (e.g., "Slasher", "Supernatural", "Psychological"). This property is used to identify the specific subgenre for which the count is being provided. The value of this property should ideally match one of the predefined subgenre tags defined in the SubgenreTag class to ensure consistency when categorizing films by their subgenres.
    /// For example, if a user has 10 films in the "Slasher" subgenre, the Subgenre property would be set to "Slasher" for that entry in the TopSubgenres list.
    /// This information can be used to generate summaries and visualizations of the user's horror film collection based on subgenre preferences.
    /// </summary>
    public required string Subgenre { get; set; }
    /// <summary>
    /// The count of horror films in the journal that belong to the specified subgenre. This property indicates how many films in the user's horror film collection are categorized under the given subgenre, providing insights into the user's preferences and trends in their viewing habits. For example, if a user has 10 films in the "Slasher" subgenre, the Count property would be set to 10 for that entry in the TopSubgenres list.
    /// </summary>
    public int Count { get; set; }
}
