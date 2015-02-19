#region using
using RedDog.Search;
using RedDog.Search.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetterAdventureWorksWeb.Helper;
using Microsoft.Owin;
using System.Collections.Specialized;
#endregion

namespace BetterAdventureWorksWeb.Model.Search
{
    /// <summary>
    /// Fabrique de SearchViewModel
    /// </summary>
    public sealed class SearchViewModelBuilder
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchClient"></param>
        public SearchViewModelBuilder(IndexQueryClient searchClient)
        {
            _searchClient = searchClient;
        } 
        #endregion

        #region Build
        public SearchViewModel Build(SearchQueryResult searchQueryResult, Uri uri)
        {
            SearchViewModel vm = new SearchViewModel();
            if (searchQueryResult == null)
            {
                vm.ErrorMessage = "Désolé, aucun résultat n'a pu être trouvé";
                return vm;
            }

            vm.Count = searchQueryResult.Count;
            vm.NextLink = GetNextLink(searchQueryResult, uri);

            foreach (var record in searchQueryResult.Records)
            {
                Product product = new Product();
                record.Properties.Populate(product);

                vm.Products.Add(product);
            }

            vm.Facets = LoadFacets(searchQueryResult, uri);

            return vm;
        }
        #endregion

        #region LoadFacets (private)
        private static Dictionary<string, List<FacetItemViewModel>> LoadFacets(SearchQueryResult searchQueryResult, Uri uri)
        {
            if (searchQueryResult.Facets == null)
            {
                return null;
            }

            UriBuilder uriBuilder = new UriBuilder(uri);
            Dictionary<string, List<FacetItemViewModel>> guidedNavigation = new Dictionary<string, List<FacetItemViewModel>>();

            foreach (string key in searchQueryResult.Facets.Keys)
            {
                FacetResult[] facetResults = searchQueryResult.Facets[key];

                List<FacetItemViewModel> facettes = new List<FacetItemViewModel>();
                foreach (FacetResult facetResult in facetResults)
                {
                    if (facetResult.Count == 0)
                    {
                        // on ignore les valeurs de facette vides
                        continue;
                    }
                    if (facetResult.Count == searchQueryResult.Count)
                    {
                        // Cette facette renvoie forcément vers la page où l'on est déjà
                        continue;
                    }

                    NameValueCollection qs = HttpUtility.ParseQueryString(uri.Query);

                    FacetItemViewModel facetItemViewModel = new FacetItemViewModel();
                    facetItemViewModel.Count = facetResult.Count;
                    string filter = "";
                    if (facetResult.Value == null)
                    {
                        // plage de valeurs
                        facetItemViewModel.Text = GetRangeDisplay(facetResult);

                        if (qs.AllKeys.Contains("filter"))
                        {
                            // le filtre n'est pas vide
                            filter = qs["filter"];
                            filter += " and ";
                        }

                        if (facetResult.From < facetResult.To)
                        {
                            filter += string.Concat(key, " ge ", facetResult.From, " and ", key, " le ", facetResult.To);
                        }
                        else
                        {
                            filter += string.Concat(key, " ge ", facetResult.From);
                        }
                    }
                    else
                    {
                        // valeur simple
                        facetItemViewModel.Text = facetResult.Value;

                        if (qs.AllKeys.Contains("filter"))
                        {
                            // le filtre n'est pas vide
                            filter += " and ";
                        }

                        filter += string.Concat(key , " eq '" , facetResult.Value , "'");
                    }
                    qs["filter"] = filter;

                    uriBuilder.Query = qs.ToString();
                    facetItemViewModel.Link = uriBuilder.Uri.ToString();

                    facettes.Add(facetItemViewModel);
                }

                if (facettes.Count > 0)
                {
                    // cette facette n'a pas de valeurs, inutile de l'afficher
                    guidedNavigation.Add(key, facettes);
                }
            }

            return guidedNavigation;
        }
        #endregion

        #region GetRangeDisplay (private, static)
        private static string GetRangeDisplay(FacetResult facetResult)
        {
            string retour;

            if (facetResult.To == 0 && facetResult.From >= 0)
            {
                retour = string.Format(templateSup, facetResult.From);
            }
            else
            {
                retour = string.Format(templateRange, facetResult.From, facetResult.To);
            }

            return retour;
        }
        const string templateRange = "{0} - {1}";
        const string templateSup = ">= {0}";
        #endregion

        #region GetNextLink (private, static)
        private static string GetNextLink(SearchQueryResult searchQueryResult, Uri uri)
        {
            string nextLink = null;

            if (searchQueryResult.Count > searchQueryResult.Records.Count())
            {
                UriBuilder uriBuilder = new UriBuilder(uri);
                NameValueCollection qs = HttpUtility.ParseQueryString(uri.Query);
                int top = 10;
                if (qs.AllKeys.Contains("top"))
                {
                    top = (string.IsNullOrEmpty(qs["top"]) ? 10 : Convert.ToInt32(qs["top"]));
                }
                top += 10;
                qs["top"] = top.ToString();
                uriBuilder.Query = qs.ToString();
                nextLink = uriBuilder.Uri.ToString();
            }

            return nextLink;
        }
        #endregion

        private IndexQueryClient _searchClient;
    }
}