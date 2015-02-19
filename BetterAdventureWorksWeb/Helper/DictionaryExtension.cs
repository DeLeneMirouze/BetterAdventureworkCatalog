#region using
using BetterAdventureWorksWeb.Model.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
#endregion

namespace BetterAdventureWorksWeb.Helper
{
    public static class DictionaryExtension
    {
        #region Value
        /// <summary>
        /// Retourne la première valeur typée d'une propriété donnée
        /// </summary>
        /// <typeparam name="T">Type de l'attribut</typeparam>
        /// <param name="clef">Nom de l'attribut</param>
        /// <returns></returns>
        public static T Value<T>(this Dictionary<string, object> dico, string clef)
            where T : struct
        {
            if (!dico.ContainsKey(clef))
            {
                return default(T);
            }

            object valeur = dico[clef];

            T val = (T)Convert.ChangeType(valeur, typeof(T));


            return val;
        }

        /// <summary>
        /// Retourne la première valeur string d'une propriété donnée
        /// </summary>
        /// <param name="clef">Nom de l'attribut</param>
        /// <returns></returns>
        public static string Value(this Dictionary<string, object> dico, string clef)
        {
            if (!dico.ContainsKey(clef))
            {
                return null;
            }

            object valeur = dico[clef];

            return valeur as string;
        }


        /// <summary>
        /// Retourne la première valeur string d'une propriété donnée
        /// </summary>
        /// <param name="clef">Nom de l'attribut</param>
        /// <returns></returns>
        public static string Value(this Dictionary<string, string[]> dico, string clef)
        {
            if (!dico.ContainsKey(clef))
            {
                return null;
            }

            string[] valeur = dico[clef];
            if (valeur.Length==0)
            {
                return null;
            }
            return valeur.First();
        } 
        #endregion

        #region Populate
        public static void Populate(this Dictionary<string, object> dico, Product product)
        {
            product.Category = dico.Value("categoryName");
            product.Color = dico.Value("color");
            product.Description = dico.Value("description");
            product.ModelName = dico.Value("modelName");
            product.Name = dico.Value("name");
            product.Price = dico.Value<decimal>("listPrice");
            product.ProductId = dico.Value<int>("productID");
            product.Size = dico.Value("size");
            product.Weight = dico.Value("weight");
        } 
        #endregion
    }
}