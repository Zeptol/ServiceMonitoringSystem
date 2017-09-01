using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class ManageController : Controller
    {
        private readonly IMongoRepository<ServiceEntity> _service;
        public ManageController(IMongoRepository<ServiceEntity> service)
        {
            _service = service;
        }
        //
        // GET: /Service/Manage/
        public ActionResult Index()
        {
            return View();
        }
	}
}