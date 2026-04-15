using Microsoft.UI.Xaml;

namespace LexArtHunt.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        // This stays empty or deleted because the main App class handles initialization
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}