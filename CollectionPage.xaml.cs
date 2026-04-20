using System.IO;
using Microsoft.Maui.Controls;
using LexArtHunt;

namespace LexArtHunt
{
    public class CollectedArtViewModel
    {
        public MyItem ArtItem { get; set; }
        public CollectedItem CollectedItem { get; set; }
        public string CollectedAtFormated => $"Collected on: {CollectedItem.CollectedAt.ToLocalTime():MMMM dd, yyyy}";
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
                await Navigation.PushAsync(new DetailsPage(vm.ArtItem));
            }
        }
    }
}
