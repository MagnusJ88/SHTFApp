using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SHTFApp
{
    public partial class App : Application
    {
        public static string DatabaseLocation;
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzkyOTg3QDMxMzgyZTM0MmUzMFFjOXUyWHJmZXpXNmJoZDFZN1BCVENIeXJJSDNFODRBdDN0OGMwd0xoTGM9");
            InitializeComponent();

            DependencyService.Get<INotificationManager>().Initialize();

            MainPage = new NavigationPage(new MainPage());
        }

        public App(string databaseLocation)
        {
            InitializeComponent();
            DependencyService.Get<INotificationManager>().Initialize();

            MainPage = new NavigationPage(new MainPage());

            DatabaseLocation = databaseLocation;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
