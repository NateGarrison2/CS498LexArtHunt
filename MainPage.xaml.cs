using LexArtHunt;
using Mapsui.UI.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;

namespace LexArtHunt;

public partial class MainPage : ContentPage
{
    private bool _isMapLoaded = false;

    public MainPage()
    {
        InitializeComponent();
        this.BackgroundColor = Colors.Transparent;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Only load the map once to prevent memory leaks if you navigate back and forth
        if (!_isMapLoaded)
        {
            await SetupMapAndPermissionsAsync();
            _isMapLoaded = true;
        }
    }

    private async Task SetupMapAndPermissionsAsync()
    {
        // 1. Initialize the free OpenStreetMap layer
        ArtMap.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        // 2. Request GPS Permissions (Crucial for 2026 Mobile Standards)
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            ArtMap.MyLocationEnabled = true; // Turn on the Blue Dot
        }

        // 3. Load the data
        await LoadPinsFromDatabaseAsync();
    }

    private async Task LoadPinsFromDatabaseAsync()
    {
        List<MyItem> artItems;

        // The .Value and ! ensure we satisfy the .NET 10 Nullable checks
        using (var db = new AppDbContext())
        {
            artItems = db.Items
                      .Where(i => i.Latitude != null && i.Longitude != null)
                      .ToList();
        }
            

        foreach (var item in artItems)
        {
            var pin = new Pin(ArtMap)
            {
                Position = new Position(item.Latitude!.Value, item.Longitude!.Value),
                Label = item.Title,
                Address = item.Artist,
                Type = PinType.Pin,
                Tag = item
            };

            ArtMap.Pins.Add(pin);
            System.Diagnostics.Debug.WriteLine($"PIN ADDED: {item.Title} at {item.Latitude}, {item.Longitude}");
        }

        // NEW .NET 10 COMPLIANT INTERACTION:
        // We listen for a tap on the MapView, then ask it which pin was touched.
        ArtMap.PinClicked += (s, e) =>
        {
            if (e.Pin != null)
            {
                e.Handled = true; // Stop the map from moving
                var selectedItem = (MyItem)e.Pin.Tag;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new DetailsPage(selectedItem));
                });
            }
        };

        // Center on Lex
        var lexington = new Position(38.0464, -84.4970);
        ArtMap.Map?.Navigator.CenterOnAndZoomTo(lexington.ToMapsui(), 2);

        ArtMap.Refresh();
    }

    private async void OnViewCollectionClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CollectionPage());
    }
}