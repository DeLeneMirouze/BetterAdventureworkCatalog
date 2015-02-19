#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
#endregion

namespace BetterAdventureWorksWeb.Model
{
    public sealed class MyViewEngine : RazorViewEngine
    {
        private static string[] NewPartialViewFormats = new[] {
        "~/Views/{1}/_{0}.cshtml",
        "~/Views/Shared/_{0}.cshtml"
    };

        public MyViewEngine()
        {
            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(NewPartialViewFormats).ToArray();
        }

    }
}