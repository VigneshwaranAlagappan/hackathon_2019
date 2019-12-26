using Bounty.Base;
using Bounty.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace hackathon_2019
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            var segments = HttpContext.Current.Request.Url;

            if (User.Identity.IsAuthenticated && (!segments.AbsolutePath.ToLower().Contains("/content/")
                                                  && !segments.AbsolutePath.ToLower().Contains("/scripts/")
                                                  && !segments.AbsolutePath.ToLower().Contains("/autodiscover/")
                                                  && !segments.AbsolutePath.ToLower().Contains("/bundles/")
                                                  && (HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".axd"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".ico"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".js"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".png"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".svg"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".jpg"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".txt"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".css"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".aspx"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".woff"
                                                      && HttpContext.Current.Request.CurrentExecutionFilePathExtension != ".gif")))
            {
                Context.User = new ServerPrincipal(HttpContext.Current.User.Identity);
                if (Context.User.Identity == null)
                {
                    FormsAuthentication.SignOut();
                    Request.GetOwinContext().Authentication.SignOut("Cookies");
                    Response.RedirectToRoute("Login");
                }

                if (string.IsNullOrWhiteSpace(Request.Url.LocalPath.TrimEnd('/')))
                {
                    Response.RedirectToRoute("Index");
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var exception = Server.GetLastError();
                var routeData = new RouteData();
            }
            catch (Exception ex)
            {
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["AppDataPath"] + "BugBounty.sdf"))
            {
                new Database().GenerateDatabase(); 
            }
        }
    }
}
