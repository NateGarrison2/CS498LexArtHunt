namespace LexArtHunt;

public partial class DetailsPage : ContentPage
{
    public DetailsPage(MyItem selectedArt)
    {
        InitializeComponent();

        TitleLabel.Text = selectedArt.Title;
        ArtistLabel.Text = "By: " + selectedArt.Artist;
        DescriptionLabel.Text = selectedArt.Description;
        ArtImage.Source = selectedArt.ImagePath;
    }
}