using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using SHTFApp.Classes;
using System.Collections.ObjectModel;

namespace SHTFApp
{
    public partial class MainPage : ContentPage
    {
        private List<Item> Items;
        private Item SelectedItem;

        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseLocation))
            {
                connection.CreateTable<Item>();
                Items = new List<Item>(connection.Table<Item>());
                itemsListView.ItemsSource = Items.OrderBy(item => item.ExpirationDate.Date);
            }
        }

        private void AddItems_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddingItemsPage());
        }

        private void ItemsListView_ItemSelected(object sender, EventArgs e)
        {
            SelectedItem = (Item)itemsListView.SelectedItem;

            if (SelectedItem != null)
            {
                Navigation.PushAsync(new AddingItemsPage(SelectedItem));
            }
        }
    }
}
