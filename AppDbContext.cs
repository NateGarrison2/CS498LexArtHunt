using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class AppDbContext : DbContext
{
    // This DbSet represents the "Items" table in the database, which contains all the public art pieces
    public DbSet<MyItem> Items { get; set; }

    // Configure the database connection to use SQLite and point to the appdata.db file in the AppData directory
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "appdata.db");

        // FORCE REFRESH: This ensures your local cache always matches your new geocoded DB
        // You can remove this line AFTER the demo if you want to save user-generated data

        /* 
        if (File.Exists(dbPath)) { File.Delete(dbPath); }
        if (!File.Exists(dbPath))
        {
            // Open the fresh geocoded DB from your project assets
            using var stream = FileSystem.OpenAppPackageFileAsync("appdata.db").Result;
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            File.WriteAllBytes(dbPath, memoryStream.ToArray());
        }
        */

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}

public class MyItem
{
    [Key]
    public int OBJECTID { get; set; }
    public string? District { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Artist { get; set; }
    public string? Category { get; set; }
    public string? YearCreated { get; set; }
    public string? Address { get; set; }
    public string? Location { get; set; }
    public string? OwnedBy { get; set; }
    public string? GlobalID { get; set; }
    public string? Viewable { get; set; }
    public string? ImagePath { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

}