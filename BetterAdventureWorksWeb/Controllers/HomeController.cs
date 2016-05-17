#region using
using BetterAdventureWorksWeb.Business;
using BetterAdventureWorksWeb.Model;
using BetterAdventureWorksWeb.Model.Search;
using RedDog.Search;
using RedDog.Search.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc; 
#endregion

namespace BetterAdventureWorksWeb.Controllers
{
    public sealed class HomeController : MyController
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="suggestViewModelBuilder"></param>
        /// <param name="catalogSearch"></param>
        /// <param name="detailViewModelBuilder"></param>
        /// <param name="searchViewModelBuilder"></param>
        public HomeController(SearchViewModelBuilder searchViewModelBuilder, CatalogSearch catalogSearch, SuggestViewModelBuilder suggestViewModelBuilder, DetailViewModelBuilder detailViewModelBuilder)
        {
            _searchViewModelBuilder = searchViewModelBuilder;
            _catalogSearch = catalogSearch;
            _suggestViewModelBuilder = suggestViewModelBuilder;
            _detailViewModelBuilder = detailViewModelBuilder;
        }
        #endregion

        #region Index
        public ActionResult Index()
        {
            return View();
        } 
        #endregion

        #region Search
        public async Task<ActionResult> Search([FromUri]SearchQuery searchQuery)
        {
            ViewBag.Query = searchQuery.Query;
            ViewBag.Sort = searchQuery.OrderBy;

            SearchQueryResult searchQueryResult = await _catalogSearch.Search(searchQuery);
            SearchViewModel vm = _searchViewModelBuilder.Build(searchQueryResult, Request.Url);

            return View(vm);
        } 
        #endregion

        #region Suggest
        /// <summary>
        /// Autocompletion
        /// </summary>
        /// <param name="term">terme saisi</param>
        /// <returns></returns>
        public async Task<ActionResult> Suggest(string term)
        {
            var retour = await _catalogSearch.Suggestion(term);
            List<string> suggestions = _suggestViewModelBuilder.Build(retour);

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = suggestions
            };
        } 
        #endregion

        #region Detail
        public async Task<ActionResult> Detail(string productID)
        {
            LookupQuery lookupQuery = new LookupQuery(productID);
            LookupQueryResult detailQueryResult = await _catalogSearch.SearchById(lookupQuery);
            DetailViewModel vm = _detailViewModelBuilder.Build(detailQueryResult, Request.Url);

            return View(vm);
        }
        #endregion

        private readonly SearchViewModelBuilder _searchViewModelBuilder;
        private readonly CatalogSearch _catalogSearch;
        private readonly SuggestViewModelBuilder _suggestViewModelBuilder;
        private readonly DetailViewModelBuilder _detailViewModelBuilder;
    }
}