using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace LexArtHunt;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Use OnAppearing so the map is ready before we add pins
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPinsFromDatabase();
    }

    private async Task LoadPinsFromDatabase()
    {
        using var db = new AppDbContext();
        var artItems = db.Items.ToList();

        foreach (var item in artItems)
        {
            try
            {
                // 1. Geocode the address to get Coordinates
                // We add ", Lexington, KY" to ensure accuracy
                string fullAddress = $"{item.Address}, Lexington, KY";
                var locations = await Geocoding.Default.GetLocationsAsync(fullAddress);
                var location = locations?.FirstOrDefault();

                if (location != null)
                {
                    // 2. Create the Pin
                    var pin = new Pin
                    {
                        Label = item.Title,
                        Address = item.Artist, // Shows artist name under title
                        Type = PinType.Place,
                        Location = new Location(location.Latitude, location.Longitude)
                    };

                    // 3. Handle the click (Lauren/Alex's Details Story)
                    pin.MarkerClicked += async (s, e) =>
                    {
                        // For the prototype, we'll just show a popup
                        // In the final app, this would navigate to DetailsPage
                        await DisplayAlert(item.Title,
                            $"Artist: {item.Artist}\n\n{item.Description}", "Close");
                    };

                    ArtMap.Pins.Add(pin);
                }
            }
            catch (Exception ex)
            {
                // If geocoding fails for one pin, we just skip it and move to the next
                System.Diagnostics.Debug.WriteLine($"Geocoding failed for {item.Title}: {ex.Message}");
            }
        }

        // Center the map on downtown Lexington
        var lexington = new Location(38.0464, -84.4970);
        ArtMap.MoveToRegion(MapSpan.FromCenterAndRadius(lexington, Distance.FromMiles(1.5)));
    }
}