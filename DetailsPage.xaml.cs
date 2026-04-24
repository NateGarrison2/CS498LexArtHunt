using System.IO;
using Microsoft.Maui.Controls;
using LexArtHunt; // Ensure this matches where your MyItem class lives

namespace LexArtHunt;

public partial class DetailsPage : ContentPage
{
    private readonly MyItem _item;
    public DetailsPage(MyItem item)
    {
        _item = item;
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

        UpdateCollectButton();
    }

    private void UpdateCollectButton()
    {
        using var userDb = new UserDbContext();
        bool alreadyCollected = userDb.CollectedItems.Any(c => c.ArtItemObjectId == _item.OBJECTID);
        CollectButton.Text = alreadyCollected ? "Collected" : "Add to Collection";
        CollectButton.IsEnabled = !alreadyCollected;
    }

    private async void OnCollectClicked(object sender, EventArgs e)
    {
        using var userDb = new UserDbContext();
        if (!userDb.CollectedItems.Any(c => c.ArtItemObjectId == _item.OBJECTID))
        {
            userDb.CollectedItems.Add(new CollectedItem { ArtItemObjectId = _item.OBJECTID });
            await userDb.SaveChangesAsync();
            UpdateCollectButton();
            await DisplayAlertAsync("Collected!", $"{_item.Title} has been added to your collection.", "OK");
        }
    }
}