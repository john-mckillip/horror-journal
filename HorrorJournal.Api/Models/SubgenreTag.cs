namespace HorrorJournal.Api.Models;
/// <summary>
/// Defines a static class containing constant string values for various horror film subgenre tags (e.g., Slasher, Supernatural, Folk Horror). These constants can be used throughout the application to ensure consistency when categorizing films by their subgenres. The class also includes a read-only list of all subgenre tags for easy reference and validation purposes.
/// </summary>
public static class SubgenreTag
{
    /// <summary>
    /// The "Slasher" subgenre tag, typically used to categorize horror films that involve a killer stalking and murdering a group of people, often with bladed tools. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal.
    /// </summary>
    public const string Slasher = "Slasher";
    /// <summary>
    /// The "Supernatural" subgenre tag, used to categorize horror films that involve elements beyond the natural world, such as ghosts, demons, or other paranormal phenomena. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal based on supernatural themes.
    /// </summary>
    public const string Supernatural = "Supernatural";
    /// <summary>
    /// The "Folk Horror" subgenre tag, used to categorize horror films that draw on folklore, rural settings, and traditional beliefs to create a sense of dread and unease. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal based on folk horror themes.
    /// </summary>
    public const string FolkHorror = "Folk Horror";
    /// <summary>
    /// The "Body Horror" subgenre tag, used to categorize horror films that focus on the grotesque and disturbing transformation or mutilation of the human body. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal based on body horror themes.
    /// </summary>
    public const string BodyHorror = "Body Horror";
    /// <summary>
    /// The "Cosmic Horror" subgenre tag, used to categorize horror films that explore themes of existential dread, the insignificance of humanity, and the presence of incomprehensible cosmic entities. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal based on cosmic horror themes.
    /// </summary>
    public const string CosmicHorror = "Cosmic Horror";
    /// <summary>
    /// The "Psychological" subgenre tag, used to categorize horror films that focus on the mental and emotional states of characters, often exploring themes of fear, paranoia, and psychological trauma. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal based on psychological horror themes.
    /// </summary>
    public const string Psychological = "Psychological";
    /// <summary>
    /// The "Found Footage" subgenre tag, used to categorize horror films that are presented as if they were discovered recordings, often shot in a documentary style. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal based on found footage themes.
    /// </summary>
    public const string FoundFootage = "Found Footage";
    /// <summary>
    /// The "J-Horror" subgenre tag, used to categorize horror films that originate from Japan and often feature themes of vengeful spirits, curses, and psychological horror. This tag can be applied to films that fit this specific subgenre to help users identify and filter their horror film entries in the journal based on J-Horror themes.
    /// </summary>
    public const string JHorror = "J-Horror";
    /// <summary>
    /// A read-only list of all subgenre tags defined in this class. This list can be used for validation purposes, ensuring that any subgenre tags applied to films in the journal are consistent with the predefined set of tags. It also provides an easy reference for users when categorizing their horror film entries by subgenre.
    /// </summary>
    public static readonly IReadOnlyList<string> All =
    [
        Slasher,
        Supernatural,
        FolkHorror,
        BodyHorror,
        CosmicHorror,
        Psychological,
        FoundFootage,
        JHorror
    ];
}
