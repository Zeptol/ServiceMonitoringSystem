using System.Web.Mvc;
using System.Web.Routing;
using FineUIMvc;
using Newtonsoft.Json.Linq;

namespace ServiceMonitoringSystem.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            ModelBinders.Binders.Add(typeof(JArray), new JArrayModelBinder());
            ModelBinders.Binders.Add(typeof(JObject), new JObjectModelBinder());

            AutofacConfig.Register();
        }
    }
}
