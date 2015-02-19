#region using
using RedDog.Search.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
#endregion

namespace BetterAdventureWorksWeb.Model.Search
{
    /// <summary>
    /// Vue/modèle de la page de recherche
    /// </summary>
    public sealed class SearchViewModel
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public SearchViewModel()
        {
            Products = new List<Product>();
        } 
        #endregion

        public string ErrorMessage { get; set; }

        public int Count { get; set; }

        public string NextLink { get; set; }

        public List<Product> Products { get; set; }

        public Dictionary<string,List<FacetItemViewModel>> Facets { get; set; }
    }
}