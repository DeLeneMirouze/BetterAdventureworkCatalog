#region using
using RedDog.Search.Http;
using RedDog.Search.Model;
using System;
using System.Threading.Tasks; 
#endregion

namespace BetterAdventureWorksWeb.Repository
{
    public interface ICatalogSearchRepository
    {
        Task<IApiResponse<SuggestionResult>> Suggestion(string indexName, string term);
        Task<IApiResponse<SearchQueryResult>> Search(string indexName, SearchQuery searchQuery);
        Task<IApiResponse<LookupQueryResult>> Lookup(string indexName, LookupQuery lookupQuery);
    }
}
