﻿#region using
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using System;
using System.Threading.Tasks;

#endregion

namespace BetterAdventureWorksWeb.Repository
{
    public sealed class CatalogSearchRepository : ICatalogSearchRepository, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchClient"></param>
        public CatalogSearchRepository(IndexQueryClient searchClient)
        {
            _searchClient = searchClient;
        } 
        #endregion

        #region Suggestion
        /// <summary>
        /// Lance une recherche de type autocomplétion dans l'indexe Azure Search
        /// </summary>
        /// <param name="indexName">Nom du catalogue dans lequel lancer la recherche</param>
        /// <param name="term"></param>
        public async Task<IApiResponse<SuggestionResult>> Suggestion(string indexName, string term)
        {
            SuggestionQuery suggestionQuery = new SuggestionQuery(term);
            var result =  await _searchClient.SuggestAsync(indexName, suggestionQuery);
            return result;
        } 
        #endregion

        #region Search
        /// <summary>
        /// Lance une recherche
        /// </summary>
        /// <param name="indexName">Nom du catalogue dans lequel lancer la recherche</param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public async Task<IApiResponse<SearchQueryResult>> Search(string indexName, SearchQuery searchQuery)
        {
            var result = _searchClient.SearchAsync(indexName, searchQuery);
            return await result;
        } 
        #endregion

        #region Lookup
        /// <summary>
        /// Recherche les détails d'un produit
        /// </summary>
        /// <param name="indexName">Nom du catalogue dans lequel lancer la recherche</param>
        /// <param name="lookupQuery"></param>
        /// <returns></returns>
        public async Task<IApiResponse<LookupQueryResult>> Lookup(string indexName, LookupQuery lookupQuery)
        {
            var result = _searchClient.LookupAsync(indexName, lookupQuery);
            return await result;
        }
        #endregion

        private readonly IndexQueryClient _searchClient;

        #region Dispose
        public void Dispose()
        {
            if (_searchClient != null)
            {
                _searchClient.Dispose();
            }
        } 
        #endregion
    }
}