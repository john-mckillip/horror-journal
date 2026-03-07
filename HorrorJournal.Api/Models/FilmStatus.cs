namespace HorrorJournal.Api.Models;
/// <summary>
/// Defines the possible statuses for a horror film in the journal, indicating whether the film has been watched, is on the user's wishlist, or has been abandoned. This enumeration is used to categorize films based on the user's interaction with them.
/// </summary>
public enum FilmStatus
{
    Watched,
    Wishlist,
    Abandoned
}
