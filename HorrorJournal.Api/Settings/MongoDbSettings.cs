namespace HorrorJournal.Api.Settings;

public class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}
