#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
#endregion

namespace BetterAdventureWorksWeb.Model.Search
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public string Category { get; set; }
        public string ModelName { get; set; }
        public string Description { get; set; }
    }
}