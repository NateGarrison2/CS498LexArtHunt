using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace LexArtHunt;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Seed appdata.db once at startup
        string dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "appdata.db");
        if (File.Exists(dbPath)) File.Delete(dbPath);
        using var stream = FileSystem.OpenAppPackageFileAsync("appdata.db").Result;
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        File.WriteAllBytes(dbPath, ms.ToArray());

        // Ensure the User Database and schema exists on first launch
        using var userDB = new UserDbContext();
        userDB.Database.EnsureCreated();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}