#region using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text; 
#endregion

namespace BetterIndexer
{
    [DebuggerDisplay("{Name}")]
    sealed class Product
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public string Color { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string Size { get; set; }
        public decimal? Weight { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public TimeSpan? DiscontinuedDate { get; set; }
        public string CategoryName { get; set; }
        public string ModelName { get; set; }
        public string Description { get; set; }
        public string DescriptionFr { get; set; }
        public int Booster { get; set; }
    }
}
