using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace SHTFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        public delegate void SetBarcodeEventHandler(object source, EventArgs args);
        public event SetBarcodeEventHandler SetBarcode;

        public ScanPage()
        {
            InitializeComponent();

            _zxing = new ZXingScannerView();
        }

        protected virtual void OnBarcodeScanned()
        {
            SetBarcode?.Invoke(this, EventArgs.Empty);
        }
        public void ZXingScannerView_OnScanResult(ZXing.Result result)
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                AddingItemsPage._scannedBarcode = result.Text;
                await DisplayAlert("Scan result", result.Text, "OK");
                OnBarcodeScanned();
                await Navigation.PopAsync();
            });

        }
    }
}