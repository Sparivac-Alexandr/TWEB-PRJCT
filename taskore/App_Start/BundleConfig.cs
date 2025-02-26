using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace taskore.App_Start
{
	public class BundleConfig
	{
        public static void RegisterBundles(BundleCollection bundles) {

            //Style Bundle
            bundles.Add(new StyleBundle("~/Content/css").Include(
            "~/Content/static/site.css"));


            //Script Bundle
            bundles.Add(new ScriptBundle("~/bundles/sitejs").Include(
            "~/Content/static/site.js"));


        }


    }
}