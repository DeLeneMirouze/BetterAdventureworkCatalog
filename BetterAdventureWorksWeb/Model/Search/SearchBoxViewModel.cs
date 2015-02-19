#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
#endregion

namespace BetterAdventureWorksWeb.Model.Search
{
    public sealed class SearchBoxViewModel
    {
        /// <summary>
        /// Chaîne recherchée
        /// </summary>
        public string SearchString { get; set; }

        public string Color { get; set; }

        public string Category { get; set; }

        public string PriceFrom { get; set; }

        public string PriceTo { get; set; }

        public string Sort { get; set; }
    }
}