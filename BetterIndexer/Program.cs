#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
#endregion

namespace BetterIndexer
{
    // http://blog.xebia.fr/2014/03/17/post-vs-put-la-confusion/

    class Program
    {
        #region Main
        static void Main(string[] args)
        {
            _serviceUri = new Uri("https://" + ConfigurationManager.AppSettings["SearchServiceName"] + ".search.windows.net");
            string indexName = "catalog";

            using (AzureSearchHelper azureSearchHelper = new AzureSearchHelper())
            {
                try
                {
                    Console.WriteLine("Supprime l'index");
                    DeleteCatalogIndex(azureSearchHelper, indexName);

                    Console.WriteLine("Création de l'index");
                    CreateCatalogIndex(azureSearchHelper, indexName);

                    Console.WriteLine("Alimentation de l'index");
                    LoadDate(azureSearchHelper, indexName);
                }
                catch (Exception e)
                {
                    while (e != null)
                    {
                        Console.WriteLine("\t{0}", e.Message);
                        e = e.InnerException;
                    }

                    Console.WriteLine();
                    Console.WriteLine("{0}", "\nDid you remember to paste your service URL and API key into App.config?\n");
                }
            }

            Console.WriteLine("Enter pour terminer");
            Console.ReadLine();
        } 
        #endregion

        #region LoadDate (private)
        private static void LoadDate(AzureSearchHelper azureSearchHelper, string indexName)
        {
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            _jsonSettings.Converters.Add(new StringEnumConverter());

            List<Product> products = new List<Product>();

            #region Load Date - see no open connection left!!!
            string sql = "SELECT * FROM Products";
            string storeConnectionString = ConfigurationManager.AppSettings["SourceSqlConnectionString"];
            using (var connection = new SqlConnection(storeConnectionString))
            {
                connection.Open();

                products = connection.Query<Product>(sql).ToList<Product>();
            }
            #endregion

            #region Push into the index - crazy this operation does not block the database until timeout!!!!
            var indexOperations = new List<Dictionary<string, object>>();

            // on injecte une liste de dictionnaires conformément aux specs:
            // https://msdn.microsoft.com/fr-fr/library/azure/dn798930.aspx

            foreach (Product product in products)
            {
                Dictionary<string, object> dico = new Dictionary<string, object>();

                dico["@search.action"] = "upload"; // il est vide, pas d'autres actions

                dico["productID"] = product.ProductID;
                dico["name"] = product.Name;
                dico["productNumber"] = product.ProductNumber;
                dico["color"] = product.Color;
                dico["standardCost"] = product.StandardCost;
                dico["listPrice"] = product.ListPrice;
                dico["size"] = product.Size;
                dico["weight"] = product.Weight;
                dico["sellStartDate"] = product.SellStartDate;
                dico["sellEndDate"] = product.SellEndDate;
                dico["discontinuedDate"] = product.DiscontinuedDate;
                dico["categoryName"] = product.CategoryName;
                dico["modelName"] = product.ModelName;
                dico["description"] = product.Description;
                dico["descriptionFR"] = product.DescriptionFr;

                // pour le scoring
                // https://msdn.microsoft.com/en-us/library/azure/dn798928.aspx#bkmk_ex
                // http://alexmang.ro/2014/09/what-are-scoring-profiles-in-azure-search/
                dico["booster"] = (product.Color == "White") ? 10 : 1;

                indexOperations.Add(dico);

                if (indexOperations.Count > 999)
                {
                    // il y a une limite à 1000
                    IndexCatalogBatch(azureSearchHelper, indexOperations, indexName);
                    indexOperations.Clear();
                }
            }

            if (indexOperations.Count > 0)
            {
                IndexCatalogBatch(azureSearchHelper, indexOperations, indexName);
            }
            #endregion
        }
        #endregion

        #region  IndexCatalogBatch (private, static)
        private static void IndexCatalogBatch(AzureSearchHelper azureSearchHelper, List<Dictionary<string, object>> changes, string indexName)
        {
            var batch = new
            {
                value = changes
            };

            Uri uri = new Uri(_serviceUri, "/indexes/" + indexName + "/docs/index");
            string json = JsonConvert.SerializeObject(batch, _jsonSettings);
            HttpResponseMessage response = azureSearchHelper.SendSearchRequest(HttpMethod.Post, uri, json);
            response.EnsureSuccessStatusCode();
        } 
        #endregion

        #region CreateCatalogIndex (private)
        private static void CreateCatalogIndex(AzureSearchHelper azureSearchHelper, string indexName)
        {
            // https://msdn.microsoft.com/en-us/library/azure/dn798941.aspx

            string json = File.ReadAllText("index.json",Encoding.UTF8);

            #region Création d'un nouvel index (POST)
            Uri uri = new Uri(_serviceUri, "/indexes");
            HttpResponseMessage response = azureSearchHelper.SendSearchRequest(HttpMethod.Post, uri, json);
            #endregion

            //#region Ajout d'un champ et création si l'ndex n'existe pas (PUT)
            //Uri uri = new Uri(_serviceUri, "/indexes/" + indexName);
            //HttpResponseMessage response = azureSearchHelper.SendSearchRequest(HttpMethod.Put, uri, json);
            //#endregion

            response.EnsureSuccessStatusCode();
        } 
        #endregion

        #region DeleteCatalogIndex (private)
        private static bool DeleteCatalogIndex(AzureSearchHelper azureSearchHelper, string indexName)
        {
            Uri uri = new Uri(_serviceUri, "/indexes/" + indexName);
            HttpResponseMessage response = azureSearchHelper.SendSearchRequest(HttpMethod.Delete, uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            response.EnsureSuccessStatusCode();
            return true;
        } 
        #endregion

        private static Uri _serviceUri;
        private static JsonSerializerSettings _jsonSettings;
    }
}
