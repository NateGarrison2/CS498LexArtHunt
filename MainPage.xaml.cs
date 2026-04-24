using LexArtHunt;
using Mapsui.UI.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using System.Linq;
using System.Threading.Tasks;

namespace LexArtHunt;

public partial class MainPage : ContentPage
{
    private bool _isMapLoaded = false;

    private List<MyItem> _allItems = new();
    private string? _activeFilter = null;
    private bool _drawerOpen = false;

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

        // NEW .NET 10 COMPLIANT INTERACTION:
        // We listen for a tap on the MapView, then ask it which pin was touched.
        ArtMap.PinClicked += (s, e) =>
        {
            if (e.Pin != null)
            {
                e.Handled = true; // Stop the map from moving
                var selectedItem = e.Pin.Tag as MyItem;

                if (selectedItem != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PushAsync(new DetailsPage(selectedItem));
                    });
                }
            }
        };

        // 3. Load the data
        await LoadPinsFromDatabaseAsync();
    }

    private async Task LoadPinsFromDatabaseAsync()
    {
        using (var db = new AppDbContext())
        {
            _allItems = db.Items
                          .Where(i => i.Latitude != null && i.Longitude != null)
                          .ToList();
        }

        var categories = _allItems
                        .Select(i => i.Category?.Trim())
                        .Where(c => !string.IsNullOrEmpty(c))
                        .Select(c => char.ToUpper(c![0]) + c.Substring(1).ToLower()) // Normalize to Title case
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList();

        BuildCategoryList(categories!);
        ApplyFilter();

        var lexington = new Position(38.0464, -84.4970);
        ArtMap.Map?.Navigator.CenterOnAndZoomTo(lexington.ToMapsui(), 2);
        ArtMap.Refresh();
    }

    private void BuildCategoryList(List<string> categories)
    {
        CategoryList.Children.Clear();

        CategoryList.Children.Add(MakeCategoryRow("All", null, categories));
        foreach (var category in categories)
        {
            CategoryList.Children.Add(MakeCategoryRow(category, category, categories));
        }
    }

    private View MakeCategoryRow(string label, string? filterValue, List<string> categories)
    {
        bool isActive = filterValue == _activeFilter;
        int count = filterValue == null
            ? _allItems.Count
            : _allItems.Count(i =>
            {
                var normalized = i.Category?.Trim();
                if (string.IsNullOrEmpty(normalized)) return false;
                normalized = char.ToUpper(normalized[0]) + normalized.Substring(1).ToLower();
                return normalized == filterValue;
            });

        Grid row = new()
        {
            ColumnDefinitions =
            [
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            ],
            Padding = new Thickness(10),
            BackgroundColor = Colors.Transparent
        };

        Label nameLabel = new()
        {
            Text = $"{label}  {count}",
            FontSize = 14,
            TextColor = isActive ? Colors.CornflowerBlue : Colors.Black,
            FontAttributes = isActive ? FontAttributes.Bold : FontAttributes.None,
            VerticalOptions = LayoutOptions.Center
        };

        Border check = new()
        {
            WidthRequest = 20,
            HeightRequest = 20,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Padding = 0,
            Stroke = isActive ? Colors.CornflowerBlue : Colors.Black,
            BackgroundColor = isActive ? Colors.CornflowerBlue : Colors.Transparent,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            Content = isActive ? new Label
            {
                Text = "✓",
                FontSize = 11,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            } : null
        };

        row.Add(nameLabel, 0, 0);
        row.Add(check, 1, 0);

        row.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                _activeFilter = filterValue;
                BuildCategoryList(categories);
                ApplyFilter();
                CloseDrawer();
                FilterPillLabel.Text = $"Filter: {label}";
            })
        });

        return row;
    }

    private void ApplyFilter()
    {
        ArtMap.Pins.Clear();

        var filtered = _activeFilter == null
            ? _allItems
            : _allItems.Where(i => string.Equals(
                i.Category?.Trim(), _activeFilter,
                StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var item in filtered)
        {
            ArtMap.Pins.Add(new Pin(ArtMap)
            {
                Position = new Position(item.Latitude!.Value, item.Longitude!.Value),
                Label = item.Title,
                Address = item.Artist,
                Type = PinType.Pin,
                Tag = item
            });
        }

        ArtMap.Refresh();
    }

    private void OnFilterPillTapped(object sender, TappedEventArgs e)
    {
        if (_drawerOpen) CloseDrawer();
        else OpenDrawer();
    }

    private void OnScrimTapped(object sender, TappedEventArgs e) => CloseDrawer();

    private void OpenDrawer()
    {
        _drawerOpen = true;
        DrawerScrim.IsVisible = true;
        FilterChevron.Rotation = 180;
        FilterDrawer.TranslateToAsync(0, 0, 250, Easing.CubicOut);
    }

    private void CloseDrawer()
    {
        _drawerOpen = false;
        DrawerScrim.IsVisible = false;
        FilterChevron.Rotation = 0;
        FilterDrawer.TranslateToAsync(0, 400, 250, Easing.CubicIn);
    }

    private async void OnViewCollectionClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CollectionPage());
    }
}