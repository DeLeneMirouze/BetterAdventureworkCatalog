#region using
using BetterAdventureWorksWeb.Repository;
using RedDog.Search.Http;
using RedDog.Search.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web; 
#endregion

namespace BetterAdventureWorksWeb.Business
{
    public sealed class CatalogSearch
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="catalogSearchRepository"></param>
        public CatalogSearch(ICatalogSearchRepository catalogSearchRepository)
        {
            _catalogSearchRepository = catalogSearchRepository;
            IndexName = ConfigurationManager.AppSettings["IndexName"];
        }
        #endregion

        #region Suggestion
        /// <summary>
        /// Auto complétion
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public IEnumerable<SuggestionResultRecord> Suggestion(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
            {
                return null;
            }

            IApiResponse<SuggestionResult> result = _catalogSearchRepository.Suggestion(IndexName, term);
            if (!result.IsSuccess)
            {
                return null;
            }

            return result.Body.Records;
        }
        #endregion

        #region Search
        /// <summary>
        /// Lance une recherche
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public SearchQueryResult Search(SearchQuery searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery.Query))
            {
                searchQuery.Query = "*";
            }
            searchQuery.Count = true;
            searchQuery.Highlight = "description";
            searchQuery.Top = (searchQuery.Top == 0) ? 10 : searchQuery.Top;
            // attention, si le nom du champ n'est pas correct, c'est toute la recherche qui échoue
            searchQuery
                .Facet("color")
                 .Facet("listPrice", "values:10|25|100|500|1000|2500")
                 .Facet("categoryName")
                 .Select("color")
                 .Select("name")
                 .Select("productID")
                 .Select("listPrice");
   
            var result = _catalogSearchRepository.Search(IndexName, searchQuery);
            if (!result.IsSuccess)
            {
                return null;
            }

            return result.Body;
        }
        #endregion

        #region SearchById
        /// <summary>
        /// Recherche les détails d'un produit
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public LookupQueryResult SearchById(LookupQuery searchQuery)
        {
            var result = _catalogSearchRepository.Lookup(IndexName, searchQuery);
            if (!result.IsSuccess)
            {
                return null;
            }

            return result.Body;
        }
        #endregion

        public readonly string IndexName;
        private ICatalogSearchRepository _catalogSearchRepository;
    }
}