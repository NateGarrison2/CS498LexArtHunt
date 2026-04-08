namespace LexArtHunt;

public partial class DetailsPage : ContentPage
{
    // We pass the 'item' from the map to this constructor
    public DetailsPage(MyItem item)
    {
        InitializeComponent();

        TitleLabel.Text = item.Title;
        ArtistLabel.Text = $"By: {item.Artist}";
        DescriptionLabel.Text = item.Description;
        YearLabel.Text = item.YearCreated;

        if (!string.IsNullOrEmpty(item.ImagePath))
        {
            // Use ONLY the filename without .jpg or .png
            // Database: "art_1_james.jpeg" -> Loading: "art_1_james"
            string resourceName = Path.GetFileNameWithoutExtension(item.ImagePath);
            ArtImage.Source = ImageSource.FromFile(resourceName);

            System.Diagnostics.Debug.WriteLine($"MAUI is looking for resource: {resourceName}");
        }
    }
}