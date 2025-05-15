using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using taskore.App_Start;

namespace taskore
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
           AreaRegistration.RegisterAllAreas();
           
           // Register Web API routes
           // Create a configuration instance
           HttpConfiguration config = new HttpConfiguration();
           // Configure routes
           WebApiConfig.Register(config);
           // Register the configuration
           GlobalConfiguration.Configuration.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );
           
           // Register MVC routes
           RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Register Bundle Table
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}