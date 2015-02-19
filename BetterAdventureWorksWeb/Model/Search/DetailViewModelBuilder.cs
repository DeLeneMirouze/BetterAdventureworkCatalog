#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetterAdventureWorksWeb.Helper;
#endregion

namespace BetterAdventureWorksWeb.Model.Search
{
    public sealed class DetailViewModelBuilder
    {
        #region Build
        public DetailViewModel Build(RedDog.Search.Model.LookupQueryResult lookupQueryResult, Uri url)
        {
            DetailViewModel detailViewModel = new DetailViewModel();
            lookupQueryResult.Properties.Populate(detailViewModel);

            return detailViewModel;
        }
        #endregion
    }
}