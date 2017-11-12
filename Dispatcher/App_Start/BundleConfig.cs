using System.Web.Optimization;

namespace Dispatcher
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/lib/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                "~/Scripts/lib/jquery.signalR-2.1.2.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/lib/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/lib/bootstrap.js",
                      "~/Scripts/lib/respond.js"));

            bundles.Add(
                new ScriptBundle("~/bundles/libs").Include(
                    // jQuery plugins
                    "~/Scripts/lib/TrafficCop.js",
                    "~/Scripts/lib/infuser.js", // depends on TrafficCop
                    "~/Scripts/lib/jquery-ui.min.js",

                    // Knockout and its plugins
                    "~/Scripts/lib/knockout-{version}.js",
                    "~/Scripts/lib/knockout.activity.js",
                    "~/Scripts/lib/knockout.asyncCommand.js",
                    "~/Scripts/lib/knockout.dirtyFlag.js",
                    "~/Scripts/lib/knockout.validation.js",
                    "~/Scripts/lib/koExternalTemplateEngine.js",

                    // Other 3rd party libraries
                    "~/Scripts/lib/underscore.js",
                    "~/Scripts/lib/amplify.*",
                    "~/Scripts/lib/toastr.js",
                 //   "~/Scripts/lib/bootstrap-switch.min.js",
                    "~/Scripts/lib/moment-with-locales.min.js",
                    "~/Scripts/lib/sammy-latest.min.js",
                    "~/Scripts/lib/knockout-{version}.js"
                    ));
            
            bundles.Add(
                new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.min.css",
                    "~/Content/bootstrap-switch.min.css",
                    "~/Content/style.css",
                    "~/Content/jquery-ui.min.css",
                    "~/Content/jquery-ui.structure.min.css",
                    "~/Content/jquery-ui.theme.min.css",
                    "~/Content/toastr.css",
                    "~/Content/toastr-responsive.css"));

            // All application JS files
            bundles.Add(new ScriptBundle("~/bundles/app")
                .IncludeDirectory("~/Scripts/app/", "*.js", searchSubdirectories: false));
            
            BundleTable.EnableOptimizations = false;
        }
    }
}
