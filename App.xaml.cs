using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace LexArtHunt;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Ensure DB is copied before the app loads the first page
        Task.Run(async () => await CopyDatabaseIfNeeded()).Wait();

        MainPage = new NavigationPage(new MainPage());
    }

    private async Task CopyDatabaseIfNeeded()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdata.db");

        if (!File.Exists(dbPath))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("appdata.db");
            using FileStream outputStream = File.OpenWrite(dbPath);
            await fileStream.CopyToAsync(outputStream);
        }
    }
}