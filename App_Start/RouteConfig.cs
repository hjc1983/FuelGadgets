using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;

namespace FGapp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //http://blogs.msdn.com/b/webdev/archive/2013/10/17/attribute-routing-in-asp-net-mvc-5.aspx
            //var constraintsResolver = new DefaultInlineConstraintResolver();
            //constraintsResolver.ConstraintMap.Add("values", typeof(ValuesConstraint));
            //routes.MapMvcAttributeRoutes( constraintsResolver);
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
             "StateRoute",
             "{state}", // URL with parameters
             new { controller = "Home", action = "Index", state = string.Empty },
             constraints: new { state = @"QLD|NSW|VIC|ACT|TAS|SA|NT|WA" }
             );


            //http://stackoverflow.com/questions/8310815/asp-net-mvc-routing-with-optional-first-parameter
            routes.MapRoute(
            "StateRouteSite",
            "{state}/site/{action}/{id}", // URL with parameters
            new { controller = "Site", action = "Index", id = UrlParameter.Optional },
            constraints: new { state = @"QLD|NSW|VIC|ACT|TAS|SA|NT|WA" }
            );
            routes.MapRoute(
             "NonStateRouteSite",
             "site/{action}/{id}", // URL with parameters
             new { controller = "Site", action = "Index", id = UrlParameter.Optional, state = string.Empty }
             );

            routes.MapRoute(
            "NonStateRouteHome",
            "{controller}/{action}/{id}",
            new { controller = "Home", action = "Index", id = UrlParameter.Optional, state = string.Empty }
            );

            routes.MapRoute(
            "StateRouteHome",
            "{state}/{controller}/{action}/{id}", // URL with parameters
            new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            constraints: new { state = @"QLD|NSW|VIC|ACT|TAS|SA|NT|WA" }
            );

            //routes.MapRoute(
            //    name: "ajax Search",
            //    url: "{controller}/SomeActionMethod/{key}",
            //    defaults: new { controller = "Home", action = "SomeActionMethod", key = UrlParameter.Optional }
            //);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
