namespace LexArtHunt;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        
        // 1. Create your starting page (MainPage)
        var mainPage = new MainPage();

        // 2. Wrap it in a NavigationPage so you can use 'Navigation.PushAsync'
        var navPage = new NavigationPage(mainPage);

        // 3. Return a new Window with your nav stack as the root
        return new Window(navPage);
        
        
    }
}