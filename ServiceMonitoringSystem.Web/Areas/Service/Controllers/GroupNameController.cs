using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class GroupNameController : Controller
    {
        private readonly IMongoRepository<GroupName> _groupName;
        private const int PageSize = 20;

        public GroupNameController(IMongoRepository<GroupName> groupName)
        {
            _groupName = groupName;
        }
        //
        // GET: /Service/GroupName/
        public ActionResult Index()
        {
            int count;
            var list = _groupName.QueryByPage(1, PageSize, out count).ToList();
            ViewBag.RecordCount = count;
            ViewBag.PageSize = PageSize;
            return View(list);
        }

        public ActionResult DoSearch()
        {
            throw new NotImplementedException();
        }
        public ActionResult Create()
        {
            throw new NotImplementedException();
        }
        public ActionResult Edit()
        {
            throw new NotImplementedException();
        }
        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}