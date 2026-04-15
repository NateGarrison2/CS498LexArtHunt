using System.IO;
using Microsoft.Maui.Controls;
using LexArtHunt; // Ensure this matches where your MyItem class lives

namespace LexArtHunt;

public partial class DetailsPage : ContentPage
{
    public DetailsPage(MyItem item)
    {
        InitializeComponent();

        // Bind the text data
        TitleLabel.Text = item.Title ?? "Unknown Title";
        ArtistLabel.Text = string.IsNullOrWhiteSpace(item.Artist) ? "Unknown Artist" : $"By: {item.Artist}";
        YearLabel.Text = string.IsNullOrWhiteSpace(item.YearCreated) ? "" : $"Year: {item.YearCreated}";
        DescriptionLabel.Text = item.Description ?? "No description available.";

        // Handle the Image safely
        if (!string.IsNullOrEmpty(item.ImagePath))
        {
            // .NET MAUI requires the name WITHOUT the extension (e.g., "art_1_james")
            string resourceId = Path.GetFileNameWithoutExtension(item.ImagePath).ToLower();
            ArtImage.Source = ImageSource.FromFile(resourceId);

            System.Diagnostics.Debug.WriteLine($"DetailsPage loading image resource: {resourceId}");
        }
    }
}