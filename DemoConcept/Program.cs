#region using
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DemoConcept
{
    class Program
    {
        #region Main
        static void Main(string[] args)
        {
            DemoOldFashion();
            //RedDogMode();

            Console.WriteLine("Faire ENTER pour terminer");
            Console.Read();
        }
        #endregion

        #region RedDogMode (private, static)
        private async static void RedDogMode()
        {
            // il faut appeler Dispose pour libérer HttpClient d'où le using
            using (ApiConnection apiConnection = ApiConnection.Create(
                ConfigurationManager.AppSettings["Azure.Search.ServiceName"],
                ConfigurationManager.AppSettings["Azure.Search.ApiKey"]
                ))
            {

                // IndexManagementClient pour les requêtes administratives
                // -------------------------------------------------------
                IndexManagementClient managementClient = new IndexManagementClient(apiConnection);

                // liste des indexes
                Console.WriteLine("Liste des indexes du service");
                var indexes = await managementClient.GetIndexesAsync();
                if (indexes.IsSuccess)
                {
                    foreach (RedDog.Search.Model.Index index in indexes.Body)
                    {
                        Console.WriteLine(index.Name);
                    }
                    Console.WriteLine();
                }
                else
                {
                    DisplayResult(indexes);
                }

                // statistiques de catalog
                Console.WriteLine("Statistiques de 'catalog'");
                var statistics = await managementClient.GetIndexStatisticsAsync("catalog");
                if (statistics.IsSuccess)
                {
                    Console.WriteLine("Documents: {0}, Stockage: {1} octets", statistics.Body.DocumentCount, statistics.Body.StorageSize);
                    Console.WriteLine();
                }
                else
                {
                    DisplayResult(statistics);
                }

                // index catalog
                var catalog = await managementClient.GetIndexAsync("catalog");
                if (catalog.IsSuccess)
                {
                    Console.WriteLine("Champs: {0}", catalog.Body.Fields.Count);
                }
                else
                {
                    DisplayResult(catalog);
                }

                // IndexQueryClient pour les requêtes de recherche
                // ------------------------------------------------

                // recherche d'un document particulier
                IndexQueryClient queryClient = new IndexQueryClient(apiConnection);
                SearchQuery query = new SearchQuery()
                    .Count(true)  // alimente la propriété Count
                    .Select("productID,name,listPrice,description")
                    .OrderBy("listPrice")
                    .Filter("listPrice lt 500 and color eq 'Blue'");

                // on recherche les vélos bleux pas trop chers
                var searchResults = await queryClient.SearchAsync("catalog", query);
                if (searchResults.IsSuccess && searchResults.Body.Records.Any())
                {
                    Console.WriteLine("Résultats de la recherche: {0} documents", searchResults.Body.Count);
                    foreach (var result in searchResults.Body.Records)
                    {
                        Console.WriteLine("{0}", result.Properties["description"]);
                        Console.WriteLine("{0}", result.Properties["listPrice"]);
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Pas de résultats");
                }

                // utilisation des facettes
                // on souhaite afficher 2 facettes:
                // color limitée aux 3 premières valeurs
                // listPrice avec un affichage par plage de prix
                query = new SearchQuery()
                .Count(true)  // alimente la propriété Count
                .Select("productID,name,listPrice,description")
                .OrderBy("listPrice desc")
                .Facet("color", "count:3")
                .Facet("listPrice","values:200|500|1000|7000");

                searchResults = await queryClient.SearchAsync("catalog", query);
                if (searchResults.IsSuccess && searchResults.Body.Records.Any())
                {
                    Console.WriteLine("Résultats de la recherche avec affichage des facettes: {0} documents", searchResults.Body.Count);
                    foreach (var result in searchResults.Body.Records)
                    {
                        Console.WriteLine("{0}", result.Properties["description"]);
                        Console.WriteLine("{0}", result.Properties["listPrice"]);
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Pas de résultats");
                }
            }
        }

        private static void DisplayResult(IApiResponse response)
        {
            Console.WriteLine("Code: {0}, Message: {1}", response.StatusCode, response.Error.Message);
        }
        #endregion

        #region DemoOldFashion (private, static)
        /// <summary>
        /// Accéder à Azure Search avec WebRequest
        /// </summary>
        private static void DemoOldFashion()
        {
            AzureSearch azureSearch = new AzureSearch();
            WebResponse response = azureSearch.SendRequest(HttpMethod.Get.Method, "/indexes/catalog");
            if (response == null)
            {
                Console.WriteLine("L'opération ne s'est pas bien passée.");
            }
            else
            {
                // affiche la structure de l'index
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string rep = sr.ReadToEnd().Trim();

                Console.WriteLine(rep);
            }

            Console.WriteLine();
            // on recherche les documents avec le mot 'bike' dans un des champs cherchables
            // on remonte quelques facette et on se limite aux 10 premiers résultats
            response = azureSearch.SendRequest(HttpMethod.Get.Method, "/indexes/catalog/docs?count=true&search=bike&facet=color&facet=listPrice,values:10|25|100|500|1000|2500&$top=10");
            if (response == null)
            {
                Console.WriteLine("La recherche a échouée.");
            }
            else
            {
                // affiche la structure de l'index
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string rep = sr.ReadToEnd().Trim();

                Console.WriteLine(rep);
            }
        }
        #endregion
    }
}
