using System.Web.Mvc;

namespace ServiceMonitoringSystem.Web.Areas.TerminalStats
{
    public class TerminalStatsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TerminalStats";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TerminalStats_default",
                "TerminalStats/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}