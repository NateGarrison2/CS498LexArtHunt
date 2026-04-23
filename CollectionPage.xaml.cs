using System.IO;
using Microsoft.Maui.Controls;
using LexArtHunt;

namespace LexArtHunt
{
    public class CollectedArtViewModel
    {
        public MyItem? ArtItem { get; set; }
        public CollectedItem? CollectedItem { get; set; }
        public string CollectedAtFormated => $"Collected on: {CollectedItem?.CollectedAt.ToLocalTime():MMMM dd, yyyy}";
    }
    public partial class CollectionPage : ContentPage
    {
        public CollectionPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Load the collection when the page appears
            LoadCollectedArt();
        }

        // Load or refresh the collection view items
        private void LoadCollectedArt()
        {
            using var userDb = new UserDbContext();
            using var appDb = new AppDbContext();

            var collectedIds = userDb.CollectedItems
                .Select(c => c.ArtItemObjectId)
                .ToHashSet();

            var collectedArt = appDb.Items
                .Where(i => collectedIds.Contains(i.OBJECTID))
                .ToList();

            var collectedArtViewModels = collectedArt.Select(i => new CollectedArtViewModel
            {
                ArtItem = i,
                CollectedItem = userDb.CollectedItems.First(c => c.ArtItemObjectId == i.OBJECTID)
            }).ToList();

            CollectionView.ItemsSource = collectedArtViewModels;
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is CollectedArtViewModel vm)
            {
                // Clear selection so the item can be tapped again on return
                CollectionView.SelectedItem = null;
                if (vm.ArtItem != null)
                {
                    await Navigation.PushAsync(new DetailsPage(vm.ArtItem));
                }
            }
        }

        // Delete entire collection
        private async void OnDeleteCollectionClicked(object sender, EventArgs e)
        {
            using var userDb = new UserDbContext();

            // Warn the user before deleting and confirm the action
            bool answer = await DisplayAlertAsync("Confirm", "Are you sure you want to delete your entire collection?", "Yes", "No");
            if (answer)
            {
                if(!userDb.CollectedItems.Any())
                {
                    await DisplayAlertAsync("Notice!", "Your collection is already empty.", "OK");
                    return;
                }
                userDb.CollectedItems.RemoveRange(userDb.CollectedItems);
                await userDb.SaveChangesAsync();
                LoadCollectedArt(); // Refresh the collection view
                await DisplayAlertAsync("Success", "Successfully deleted entire collection.", "OK");
            }
        }
    }
}
