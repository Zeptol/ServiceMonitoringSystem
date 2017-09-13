using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using FineUIMvc;
using MongoDB.Bson;
using MongoDB.Driver;
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
            Updated += ManageController_Updated;
        }

        private void ManageController_Updated(NameValueCollection values)
        {
            var host = values["ddlHost"];
            var key = values["tbxKey"];
            var appr = values["rblApprove"];
            var filter = new List<FilterDefinition<ServiceEntity>>();
            if (!string.IsNullOrEmpty(host))
                filter.Add(Builders<ServiceEntity>.Filter.Eq(t => t.Host, host));
            if (!string.IsNullOrEmpty(key))
                filter.Add(
                    Builders<ServiceEntity>.Filter.Or(
                        Builders<ServiceEntity>.Filter.Regex(t => t.ServiceName,
                            new BsonRegularExpression(new Regex(key, RegexOptions.IgnoreCase))),
                        Builders<ServiceEntity>.Filter.Regex(t => t.SecondaryName,
                            new BsonRegularExpression(new Regex(key, RegexOptions.IgnoreCase)))));
            if (!string.IsNullOrEmpty(appr))
                filter.Add(Builders<ServiceEntity>.Filter.Eq(t => t.IsApproved, appr == "1"));
            UpdateGrid(values, filter);
        }

        //
        // GET: /Service/Manage/
        public override ActionResult Index()
        {
            var hostList = new List<ListItem>
            {
                new ListItem("全部", "", true)
            };
            hostList.AddRange(Rep.Distinct(t => t.Host).Select(t => new ListItem(t, t)));
            ViewBag.ddlHost = hostList.ToArray();
            return base.Index();
        }

        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }

         [HttpPost]
        public ActionResult Grid1_Sort(FormCollection values)
        {
            OnUpdated(values);
            return UIHelper.Result();
        }
    }
}