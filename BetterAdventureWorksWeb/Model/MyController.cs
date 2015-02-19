#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
#endregion

namespace BetterAdventureWorksWeb.Model
{
    /// <summary>
    /// Classe de base des contrôlleurs
    /// </summary>
    public abstract class MyController : Controller
    {
        #region Controller (protected)
        /// <summary>
        /// Controller
        /// </summary>
        protected MyController()
        {

        } 
        #endregion
    }
}