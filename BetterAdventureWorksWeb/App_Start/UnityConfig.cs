#region using
using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using RedDog.Search.Http;
using Microsoft.WindowsAzure;
using BetterAdventureWorksWeb.Repository; 
#endregion

namespace BetterAdventureWorksWeb.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();


            container.RegisterType<ApiConnection>(
                new InjectionFactory(c =>
                    ApiConnection.Create(
                    CloudConfigurationManager.GetSetting("Azure.Search.ServiceName"),
                    CloudConfigurationManager.GetSetting("Azure.Search.ApiKey"))));

            container.RegisterType<ICatalogSearchRepository, CatalogSearchRepository>();

        }
    }
}


// http://www.wiktorzychla.com/2013/03/unity-and-http-per-request-lifetime.html
// http://stackoverflow.com/questions/5187562/mvc-ef-datacontext-singleton-instance-per-web-request-in-unity

