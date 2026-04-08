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
    public string District { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Artist { get; set; }
    public string Category { get; set; }
    public string YearCreated { get; set; }
    public string Address { get; set; }
    public string Location { get; set; } // This is the descriptive location
    public string OwnedBy { get; set; }
    public string GlobalID { get; set; }
    public string Viewable { get; set; }
    public string ImagePath { get; set; }
}