using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class UserDbContext : DbContext
{
    public DbSet<CollectedItem> CollectedItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Separate file from appdata.db so user data is never wiped
        string dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "userdata.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}

public class CollectedItem
{
    [Key]
    public int Id { get; set; }
    public int ArtItemObjectId { get; set; }   // matches MyItem.OBJECTID
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}