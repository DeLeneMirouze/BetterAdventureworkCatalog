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
        IApiResponse<SuggestionResult> Suggestion(string indexName, string term);
        IApiResponse<SearchQueryResult> Search(string indexName, SearchQuery searchQuery);
        IApiResponse<LookupQueryResult> Lookup(string indexName, LookupQuery lookupQuery);
    }
}
