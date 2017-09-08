using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;
using ServiceMonitoringSystem.Web.Controllers;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class ManageController : BaseController<ServiceEntity>
    {
        public ManageController(IMongoRepository<ServiceEntity> rep):base(rep)
        {
            Rep = rep;
        }
        //
        // GET: /Service/Manage/
        public ActionResult Index()
        {
            return View();
        }
	}
}