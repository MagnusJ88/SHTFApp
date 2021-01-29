    using SHTFApp.Classes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SHTFApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class calculate : ContentPage
    {
        private double totalEnergySpent;
        private List<Item> Items;

        public calculate()
        {
            InitializeComponent();

            using (SQLiteConnection connection = new SQLiteConnection(App.DatabaseLocation))
            {
                connection.CreateTable<Item>();
                Items = new List<Item>(connection.Table<Item>());
                //itemsListView.ItemsSource = Items.OrderBy(item => item.ExpirationDate.Date);
            }
        }
        private void iconButton_Clicked(object sender, EventArgs e)
        {
            Launcher.OpenAsync("https://www.livsmedelsverket.se/livsmedel-och-innehall/naringsamne/energi-kalorier");
        }

        private void adultStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            adultValueLabel.Text = $"Adults: {adultStepper.Value}";
            CaloriesSpentCalculation();
        }

        private void childStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            childValueLabel.Text = $"Children: {childStepper.Value}";
            CaloriesSpentCalculation();
        }

        private void calculateButton_Clicked(object sender, EventArgs e)
        {
            Calculation();
        }
        private void Calculation()
        {
            double totalCalories = 0;
            foreach  (var item in Items)
            {
                totalCalories += (((Convert.ToDouble(item.Energy) / 100) * Convert.ToDouble(item.Quantity)) * item.Amount);
            }
            double days = totalCalories / totalEnergySpent;

            summaryLabel.Text = $"Your inventory contains a total of {totalCalories} calories. It will last for aprox {Math.Floor(days)} days.";

        }
        private void CaloriesSpentCalculation()
        {
            if (adultStepper.Value == 0 && childStepper.Value == 0)
            {
                totalCaloriesSpent.Text = $"Tot. kcal spent/day: 0";
                totalEnergySpent = 0;
            }
            else if (adultStepper.Value > 0 && childStepper.Value == 0)
            {
                totalEnergySpent = adultStepper.Value * Convert.ToDouble(adultCaloriesEntry.Text);
                totalCaloriesSpent.Text = $"Tot. kcal spent/day: {totalEnergySpent}";
            }
            else if (adultStepper.Value == 0 && childStepper.Value > 0)
            {
                totalEnergySpent = childStepper.Value * Convert.ToDouble(childCaloriesEntry.Text);
                totalCaloriesSpent.Text = $"Tot. kcal spent/day: {totalEnergySpent}";
            }
            else
            {
                totalEnergySpent = (adultStepper.Value * Convert.ToDouble(adultCaloriesEntry.Text)) + (childStepper.Value * Convert.ToDouble(childCaloriesEntry.Text));
                totalCaloriesSpent.Text = $"Tot. kcal spent/day: {totalEnergySpent}";
            }
        }
    }
}