using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using Stripe;
using System.Configuration;

namespace TeamTwoBe
{
    public class Global : HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["sk_test_P6m1FrtIXMp4Eb8vxViEhofQ00VVKk9gpa"]);

        }
    }
}