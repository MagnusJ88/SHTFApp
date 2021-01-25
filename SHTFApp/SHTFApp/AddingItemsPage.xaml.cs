using Newtonsoft.Json;
using Plugin.LocalNotification;
using RestSharp;
using SHTFApp.Classes;
using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OpenFoodFacts4Net.Json.Data;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using ZXing.Net.Mobile.Forms;

namespace SHTFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddingItemsPage : ContentPage
    {
        ZXingScannerView _zxing;
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

                    AlertMessages(rowsAdded);
                }

               /* var list = new List<string>
            {
                typeof(NotificationPage).FullName
                
            };
                var serializeReturningData = ObjectSerializer.SerializeObject(list);

                var notification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = "Detta är title",
                    Description = "Detta är Description",
                    ReturningData = serializeReturningData,
                    //NotifyTime = DateTime.Now.AddSeconds(5) //ändra till utgångsdatum
                };
                NotificationCenter.Current.Show(notification);*/
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
                DisplayAlert("Deleted", "The item has been deleted", "Ok");
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

        private void scannerButton_Clicked(object sender, EventArgs e)
        {
            _zxing = new ZXingScannerView
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 250,
                WidthRequest = 250,
                IsAnalyzing = true,
                IsScanning = true,
            };
        }

        private void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                eanEntry.Text = result.Text;
                getNutriments(result.Text);
                _zxing.IsScanning = false;
            });
        }
        private void getNutriments(string barcode)
        {
            var client = new RestClient($"https://world.openfoodfacts.org/api/v0/product/" + barcode);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse restResponse = client.Execute(request);
            dynamic newProduct = JsonConvert.DeserializeObject(restResponse.Content);

            int energy = newProduct["product"]["nutriments"]["energy-kcal"];

            string name = newProduct["product"]["product_name"];
            nameEntry.Text = name;
            energyEntry.Text = energy.ToString();
        }
    }
}