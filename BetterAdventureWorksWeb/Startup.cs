#region using
using BetterAdventureWorksWeb;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
#endregion


[assembly: OwinStartup(typeof(Startup))]

namespace BetterAdventureWorksWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}