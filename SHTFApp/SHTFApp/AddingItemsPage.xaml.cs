using Newtonsoft.Json;
using RestSharp;
using SHTFApp.Classes;
using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;

namespace SHTFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddingItemsPage : ContentPage, INotifyPropertyChanged
    {
        private INotificationManager notificationManager;
        private Item SelectedItem;

        public static string _scannedBarcode;
        public static string ScannedBarcode
        {
            get => _scannedBarcode;
            set => _scannedBarcode = value;
        }
        public AddingItemsPage()
        {
            InitializeComponent();

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
        }
        public AddingItemsPage(Item selectedItem)
        {
            InitializeComponent();

            SelectedItem = selectedItem;

            nameEntry.Text = SelectedItem.Name;
            amountEntry.Text = SelectedItem.Amount.ToString();
            expirePicker.Date = SelectedItem.ExpirationDate.Date;
            energyEntry.Text = SelectedItem.Energy.ToString();
            quantityEntry.Text = SelectedItem.Quantity.ToString();

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedItem == null)
            {
                Item item = new Item()
                {
                    Name = nameEntry.Text.ToUpper(),
                    Amount = Convert.ToDouble(amountEntry.Text),
                    ExpirationDate = expirePicker.Date,
                    Energy = Convert.ToInt32(energyEntry.Text),
                    Quantity = Convert.ToInt32(quantityEntry.Text)
                };

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Item>();
                    int rowsAdded = conn.Insert(item);

                    AlertMessages(rowsAdded);
                }

                string title = $"Expired items!";
                string message = $"You have expired items in your inventory";
                notificationManager.SendNotification(title, message, expirePicker.Date);
            }
            else
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Item>();

                    SelectedItem.Name = nameEntry.Text.ToUpper();
                    SelectedItem.Amount = Convert.ToDouble(amountEntry.Text);
                    SelectedItem.ExpirationDate = expirePicker.Date;
                    SelectedItem.Energy = Convert.ToInt32(energyEntry.Text);
                    SelectedItem.Quantity = Convert.ToInt32(quantityEntry.Text);

                    int rowsAdded = conn.Update(SelectedItem);
                    AlertMessages(rowsAdded);
                }
            }
            Navigation.PopAsync();
        }
        private void Delete_Clicked(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    //conn.CreateTable<Item>();
                    int rowsAdded = conn.Delete(SelectedItem);
                    DisplayAlert("Deleted", "The item has been deleted", "Ok");
                }
            }
            catch 
            {
                DisplayAlert("Error", "Unable to delete item", "Ok");
            }
            finally
            {
                Navigation.PopAsync();
            }
        }
        private void SearchButton_Clicked(object sender, EventArgs e)
        {
           /*var scanPage = new ScanPage();
            scanPage.SetBarcode += this.OnBarcodeScanned;
            Navigation.PushAsync(scanPage);*/

            GetNutriments(eanEntry.Text);
        }
        public void OnBarcodeScanned(object source, EventArgs e)
        {
            if (_scannedBarcode != null)
            {
                eanEntry.Text = _scannedBarcode;
                GetNutriments(_scannedBarcode);
            }
        }
        private void ImageButtonScan_Clicked(object sender, EventArgs e)
        {
            var scanPage = new ScanPage();
            scanPage.SetBarcode += this.OnBarcodeScanned;
            Navigation.PushAsync(scanPage);
        }
        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
                stackLayout.Children.Add(msg);
            });
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
        public void GetNutriments(string barcode)
        {
            var client = new RestClient($"https://sv.openfoodfacts.org/api/v0/product/" + barcode)
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            request.AddHeader("SHTFApp", Xamarin.Essentials.VersionTracking.CurrentVersion/*"Content-Type", "application/x-www-form-urlencoded"*/);
            IRestResponse restResponse = client.Execute(request);

            try
            {
                dynamic newProduct = JsonConvert.DeserializeObject(restResponse.Content);

                int energy = newProduct["product"]["nutriments"]["energy-kcal"] ?? 0;
                int quantity = newProduct["product"]["product_quantity"] ?? 0;
                string name = newProduct["product"]["product_name"] ?? "Not found";

                quantityEntry.Text = quantity.ToString();
                nameEntry.Text = name;
                energyEntry.Text = energy.ToString();
            }
            catch
            {
                DisplayAlert("Not found!", "The product was not found in the database! Add the values manually.", "OK");
                nameEntry.Focus();
            }
        }
    }
}