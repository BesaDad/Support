using Tele.Infrastructure.AutoMappingConfig;
using Tele.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Tele
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperBootStrapper.BootStrap();
            AutoMapperBootStrapper.Configure(System.Web.Mvc.DependencyResolver.Current
                    .GetServices(typeof(IAutoMapperTypeConfigurator)).Cast<IAutoMapperTypeConfigurator>());
        }
    }
}
