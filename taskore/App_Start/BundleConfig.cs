using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace taskore.App_Start
{
	public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {

            //Style Bundle
            bundles.Add(new StyleBundle("~/Content/css").Include(
     "~/wwwroot/about.css",
     "~/wwwroot/signup.css",
      "~/wwwroot/contact.css",
      "~/wwwroot/my-profile.css",
      "~/wwwroot/privacy.css",
      "~/wwwroot/terms.css",
      "~/wwwroot/home.css",
      "~/wwwroot/chat.css",
      "~/wwwroot/explore-page.css",
      "~/wwwroot/create-prjct.css"));

           //Script Bundle
            bundles.Add(new ScriptBundle("~/bundles/sitejs").Include(
            "~/wwwroot/taskore.js",
            "~/wwwroot/js/header.js"));


        }
    }
}