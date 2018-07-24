using System.Web;
using System.Web.Optimization;

namespace LaborServices.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js",
            "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                      "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/daterangepicker").Include(
                                    "~/Scripts/daterangepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                      "~/Scripts/app.js",
                       "~/Scripts/plugins/modal.view.js",
                      "~/Scripts/plugins/activate.js",
                      "~/Scripts/custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/corejs").Include(
                      "~/Scripts/moment.js",
                      "~/Scripts/selectize.js",
                      "~/Scripts/plugins/fakeLoader.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                      "~/Scripts/ckeditor/ckeditor.js"
                      ));

            #region front end
            bundles.Add(new ScriptBundle("~/bundles/frontEnd").Include(
                "~/Scripts/frontEnd/popper.min.js",
                "~/Scripts/frontEnd/bootstrap4.min.js",
                "~/Scripts/frontEnd/owl.carousel.min.js",
                "~/Scripts/frontEnd/perfect-scrollbar/jquery.mousewheel.js",
                "~/Scripts/frontEnd/perfect-scrollbar/perfect-scrollbar.js",
                //"~/Scripts/frontEnd/jquery-countTo.js",
                //"~/Scripts/frontEnd/jquery.appear.js",
                //"~/Scripts/frontEnd/mixitup.min.js",
                //"~/Scripts/frontEnd/jquery.custom-file-input.js",
                "~/Scripts/plugins/modal.view.js",
                "~/Scripts/frontEnd/plugin.js",
                "~/Scripts/frontEnd/main.js"

            ));

            //bundles.Add(new ScriptBundle("~/bundles/frontEnd2").Include(
            //));

            bundles.Add(new ScriptBundle("~/bundles/locationpicker").Include(
                "~/Scripts/locationpicker.jquery.js",
                "~/Scripts/locationpicker-ready.js",
                "~/Scripts/geolocation-marker.js"));

            #endregion

            #endregion

            // =============================================================================================================================

            #region style
            bundles.Add(new StyleBundle("~/Content/site-DalalCrm-ErrorTest").Include(
                 "~/Content/site-DalalCrm-ErrorTest.css"));

            bundles.Add(new StyleBundle("~/Content/site-DalalCrm-Terms").Include(
                  "~/Content/site-DalalCrm-Terms.css"));

            bundles.Add(new StyleBundle("~/Content/site-DalalCrm-SystemicPaymentMethod").Include(
                  "~/Content/site-DalalCrm-SystemicPaymentMethod.css"));

            bundles.Add(new StyleBundle("~/Content/site-DalalCrm-SystemicPayOnline").Include(
                  "~/Content/site-DalalCrm-SystemicPayOnline.css"));

            bundles.Add(new StyleBundle("~/Content/site-pay-online").Include(
               "~/Content/site-pay-online.css"));

            bundles.Add(new StyleBundle("~/Content/Payment-alalShopperResult").Include(
                  "~/Content/Payment-alalShopperResult.css"));
            bundles.Add(new StyleBundle("~/Content/site-payment-SystemicBankTransfer").Include(
                 "~/Content/site-payment-SystemicBankTransfer.css"));

            bundles.Add(new StyleBundle("~/Content/site-payment-PaymentMethod").Include(
                  "~/Content/site-payment-PaymentMethod.css"));

            bundles.Add(new StyleBundle("~/Content/site-payment-bankTransfer").Include(
                  "~/Content/site-payment-bankTransfer.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                  "~/Scripts/plugins/bootstrap/css/bootstrap.min.css"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/fakeLoader.css",
                      "~/Content/bootstrap-selectize.css"
                      ));


            bundles.Add(new StyleBundle("~/Content/theme").Include(
                     "~/Content/essentials1.css",
                     "~/Content/layout.css",
                     "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/datetimepicker").Include(
                      "~/Content/bootstrap-datetimepicker.css",
                        "~/Content/CheckBox-style.css"



                      ));
          

            bundles.Add(new StyleBundle("~/Content/datetimepicker-rtl").Include(
                     "~/Content/bootstrap-datetimepicker.css",
                       "~/Content/CheckBox-style-rtl.css"


                     ));

            bundles.Add(new StyleBundle("~/Content/daterangepicker").Include(
                      "~/Content/bootstrap-daterangepicker.css",
                        "~/Content/CheckBox-style.css"
                      
                      
                      ));

            bundles.Add(new StyleBundle("~/Content/rtlCss").Include(
                      "~/Scripts/plugins/bootstrap/RTL/bootstrap-rtl.css",
                      "~/Scripts/plugins/bootstrap/RTL/bootstrap-flipped.css",
                      "~/Content/layout-RTL.css" ));

            #region frontEnd

            //bundles.Add(new StyleBundle("~/FrontEnd/css").Include(
            //    "~/Content/frontEnd/bootstrap4.min.css",
            //    "~/Content/frontEnd/font-awesome.min.css",
            //    "~/Content/frontEnd/owl.carousel.min.css",
            //    "~/Content/frontEnd/animate.min.css",
            //    "~/Content/frontEnd/fileupload.css",
            //    "~/Content/frontEnd/style-ltr.css"
            //    ));

            bundles.Add(new StyleBundle("~/FrontEnd/css").Include(
                "~/Content/frontEnd/bootstrap4.min.css",
                "~/fonts/fonts.css",
                "~/Content/fakeLoader.css",
                "~/Content/bootstrap-selectize.css",
                "~/Scripts/frontEnd/perfect-scrollbar/perfect-scrollbar.css",
                "~/Content/frontEnd/style.css",
                "~/Content/frontEnd/media.css",
                "~/Content/site.css"
                ));

            bundles.Add(new StyleBundle("~/FrontEnd/rtlCss").Include(
                "~/Content/frontEnd/bootstrap4.min.css",
                "~/fonts/fonts.css",
                "~/Content/fakeLoader.css",
                "~/Content/bootstrap-selectize.css",
                 "~/Scripts/frontEnd/perfect-scrollbar/perfect-scrollbar.css",
                "~/Content/frontEnd/style-rtl.css",
                "~/Content/frontEnd/media-rtl.css",
                "~/Content/site-rtl.css"
                ));
            #endregion

            BundleTable.EnableOptimizations = false;
            #endregion
        }
    }
}
