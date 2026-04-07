using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class AppDbContext : DbContext
{
    public DbSet<MyItem> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdata.db");
        options.UseSqlite($"Filename={dbPath}");
    }
}

public class MyItem
{
    [Key]
    public int OBJECTID { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Description { get; set; }
    public string Location { get; set; } // Assuming this holds coordinates like "38.04, -84.50"
    public string ImagePath { get; set; }
}