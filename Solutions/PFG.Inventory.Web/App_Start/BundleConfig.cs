using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace PFG.Inventory.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/ace/assets/js/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/ace131").Include(
                "~/Content/ace/assets/js/ace-elements.min.js",
                "~/Content/ace/assets/js/ace.min.js"
                ));



            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
            //    "~/Scripts/knockout-{version}.js",
            //    "~/Scripts/knockout.validation.js"));

            //bundles.Add(new ScriptBundle("~/bundles/app").Include(
            //    "~/Scripts/sammy-{version}.js",
            //    "~/Scripts/app/common.js",
            //    "~/Scripts/app/app.datamodel.js",
            //    "~/Scripts/app/app.viewmodel.js",
            //    "~/Scripts/app/home.viewmodel.js",
            //    "~/Scripts/app/_run.js"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //    "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //    "~/Scripts/bootstrap.js",
            //    "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //     "~/Content/bootstrap.css",
            //     "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/ace131").Include(
                    "~/Content/ace/assets/css/bootstrap.min.css",
                    "~/Content/ace/assets/css/font-awesome.min.css",
                    "~/Content/ace/assets/css/ace-fonts.css",
                    "~/Content/ace/assets/css/ace.min.css"//ace styles
                     ));




            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
