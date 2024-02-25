namespace HaBuddies.Models;

public class HaBuddiesDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string PostCollectionName { get; set; } = null!;
}