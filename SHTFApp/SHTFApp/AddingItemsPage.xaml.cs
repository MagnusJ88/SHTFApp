using SHTFApp.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SHTFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddingItemsPage : ContentPage
    {
        Item SelectedItem;
        public AddingItemsPage()
        {
            InitializeComponent();
        }
        public AddingItemsPage(Item selectedItem)
        {
            InitializeComponent();

            SelectedItem = selectedItem;

            nameEntry.Text = SelectedItem.Name;
            amountEntry.Text = SelectedItem.Amount.ToString();
            expirePicker.Date = SelectedItem.ExpirationDate.Date;

        }

        private void saveButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedItem == null)
            {
                Item item = new Item()
                {
                    Name = nameEntry.Text.ToUpper(),
                    Amount = Convert.ToDouble(amountEntry.Text),
                    ExpirationDate = expirePicker.Date
                };

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Item>();
                    int rowsAdded = conn.Insert(item);

                    if (rowsAdded > 0)
                    {
                        DisplayAlert("Saved", "The item has been saved", "Ok");
                    }
                    else
                    {
                        DisplayAlert("Not Saved", "The item has not been saved", "Ok");
                    }
                }
            }
            else
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Item>();

                    SelectedItem.Name = nameEntry.Text.ToUpper();
                    SelectedItem.Amount = Convert.ToDouble(amountEntry.Text);
                    SelectedItem.ExpirationDate = expirePicker.Date;

                    int rowsAdded = conn.Update(SelectedItem);
                    AlertMessages(rowsAdded);
                }
            }
            Navigation.PopAsync();
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Item>();
                int rowsAdded = conn.Delete(SelectedItem);
                AlertMessages(rowsAdded);
            }
            Navigation.PopAsync();
        }
        private void AlertMessages(int added)
        {
            if (added > 0) 
            { 
                DisplayAlert("Saved", "The item has been saved", "Ok");
            }
            else
            {
                DisplayAlert("Not Saved", "The item has not been saved", "Ok");
            }
        }
    }
}