using SHTFApp.Classes;
using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SHTFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Calculate : ContentPage
    {
        private double totalEnergySpent;
        private List<Item> Items;

        public Calculate()
        {
            InitializeComponent();

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseLocation))
            {
                connection.CreateTable<Item>();
                Items = new List<Item>(connection.Table<Item>());
            }
        }
        private void IconButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Launcher.OpenAsync("https://www.livsmedelsverket.se/livsmedel-och-innehall/naringsamne/energi-kalorier");
            }
            catch
            {
                DisplayAlert("Information", "Based on numbers provided by the Swedish Food Agency", "Ok");
            }
        }

        private void AdultStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            adultValueLabel.Text = $"Adults: {adultStepper.Value}";
            CaloriesSpentCalculation();
        }

        private void ChildStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            childValueLabel.Text = $"Children: {childStepper.Value}";
            CaloriesSpentCalculation();
        }

        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            Calculation();
        }
        private void Calculation()
        {
            double totalCalories = 0;
            foreach (Item item in Items)
            {
                totalCalories += ((Convert.ToDouble(item.Energy) / 100) * Convert.ToDouble(item.Quantity)) * item.Amount;
            }
            double days = totalCalories / totalEnergySpent;

            summaryLabel.Text = $"Your inventory contains a total of {totalCalories} calories. It will last for approximately {Math.Floor(days)} days.";
        }
        private void CaloriesSpentCalculation()
        {
            if (adultStepper.Value == 0 && childStepper.Value == 0)
            {
                totalCaloriesSpent.Text = $"Total calories spent per day: 0";
                totalEnergySpent = 0;
            }
            else if (adultStepper.Value > 0 && childStepper.Value == 0)
            {
                totalEnergySpent = adultStepper.Value * Convert.ToDouble(adultCaloriesEntry.Text);
                totalCaloriesSpent.Text = $"Total calories spent per day: {totalEnergySpent}";
            }
            else if (adultStepper.Value == 0 && childStepper.Value > 0)
            {
                totalEnergySpent = childStepper.Value * Convert.ToDouble(childCaloriesEntry.Text);
                totalCaloriesSpent.Text = $"Total calories spent per day: {totalEnergySpent}";
            }
            else
            {
                totalEnergySpent = (adultStepper.Value * Convert.ToDouble(adultCaloriesEntry.Text)) + (childStepper.Value * Convert.ToDouble(childCaloriesEntry.Text));
                totalCaloriesSpent.Text = $"Total calories spent per day: {totalEnergySpent}";
            }
        }
    }
}