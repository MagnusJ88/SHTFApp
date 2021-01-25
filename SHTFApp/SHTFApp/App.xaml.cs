using Plugin.LocalNotification;
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
            InitializeComponent();

            NotificationCenter.Current.NotificationTapped += LoadPageFromNotification;

            MainPage = new NavigationPage(new MainPage());
        }

        public App(string databaseLocation)
        {
            InitializeComponent();

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
        private void LoadPageFromNotification(NotificationTappedEventArgs e)
        {

            ((NavigationPage)MainPage).Navigation.PushAsync(new MainPage());
        }
    }
}
