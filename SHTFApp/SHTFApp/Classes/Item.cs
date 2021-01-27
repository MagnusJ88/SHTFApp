using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace SHTFApp.Classes
{
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int Energy { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
