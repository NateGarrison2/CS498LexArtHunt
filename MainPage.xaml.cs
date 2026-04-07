using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace LexArtHunt;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        LoadPins();
    }

    private void LoadPins()
    {
        using var db = new AppDbContext();
        var artworks = db.Items.ToList();

        // Center map on Lexington initially
        ArtMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(38.0406, -84.5037), Distance.FromMiles(3)));

        foreach (var art in artworks)
        {
            // VERY BASIC PARSING: Assumes 'Location' string looks like "38.0406, -84.5037"
            // You may need to tweak this depending on exactly how Nate formatted the CSV!
            if (!string.IsNullOrEmpty(art.Location) && art.Location.Contains(","))
            {
                var coords = art.Location.Split(',');
                if (double.TryParse(coords[0], out double lat) && double.TryParse(coords[1], out double lon))
                {
                    var pin = new Pin
                    {
                        Label = art.Title,
                        Address = art.Artist,
                        Type = PinType.Place,
                        Location = new Location(lat, lon)
                    };

                    // Wire up the click event for the Details view
                    pin.MarkerClicked += async (s, args) =>
                    {
                        args.HideInfoWindow = true; // Stop default popups
                        await Navigation.PushAsync(new DetailsPage(art));
                    };

                    ArtMap.Pins.Add(pin);
                }
            }
        }
    }
}