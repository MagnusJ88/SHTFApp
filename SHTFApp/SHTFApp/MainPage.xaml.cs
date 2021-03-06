﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using SQLite;
using SHTFApp.Classes;

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
                SfitemsListView.ItemsSource = Items.OrderBy(item => item.ExpirationDate.Date);
            }
        }




        private void AddItems_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddingItemsPage());
        }

        private void ItemsListView_ItemSelected(object sender, EventArgs e)
        {
            if (SelectedItem != null)
            {
                Navigation.PushAsync(new AddingItemsPage(SelectedItem));
            }
        }

        private void Summary_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Calculate());
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
        }

        private void SfitemsListView_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        {
            SelectedItem = (Item)SfitemsListView.SelectedItem;
            if (SelectedItem != null)
            {
                Navigation.PushAsync(new AddingItemsPage(SelectedItem));
            }
        }
    }
}


