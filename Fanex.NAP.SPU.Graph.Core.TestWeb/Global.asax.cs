using Fanex.NAP.SPU.Graph.Core.Neo4j;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Fanex.NAP.SPU.Graph.Core.TestWeb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DbSessionFactory.Init()
                .RegisterConfigs("GraphDB")
                .RegisterXmlFile("~/App_Data/Graph", "Script.xml")
                .RegisterDriverFactory(new Neo4jDriverFactory())
                .RegisterCommandBuilder(new Neo4jCommandBuilder("~/App_Data/Graph/scripts"))
                .RegisterCommandExecutor(new Neo4jExecutor())
                .Run();
        }
    }
}
