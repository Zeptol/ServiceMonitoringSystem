using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using FineUIMvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceMonitoringSystem.Common.Enums;
using ServiceMonitoringSystem.Interface;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;
using ServiceMonitoringSystem.Web.Controllers;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class ManageController : BaseController<ServiceEntity>
    {
        private readonly ITree _tree;
        private static FilterDefinition<ServiceEntity> _typeFilter;

        public ManageController(IMongoRepository<ServiceEntity> rep, ITree tree) : base(rep)
        {
            _tree = tree;
            Rep = rep;
            Updated += ManageController_Updated;
        }

        private void ManageController_Updated(NameValueCollection values)
        {
            var host = values["ddlHost"];
            var key = values["tbxKey"];
            var appr = values["rblApprove"];
            var nodeId = values["nodeId"];
            var filter = new List<FilterDefinition<ServiceEntity>>();
            if (_typeFilter != null) filter.Add(_typeFilter);
            if (!string.IsNullOrEmpty(nodeId))
                if (nodeId != "all")
                    if (nodeId.StartsWith("1"))
                        filter.Add(Builders<ServiceEntity>.Filter.Eq(t => t.PrimaryId, int.Parse(nodeId.Substring(2))));
                    else if (nodeId.StartsWith("2"))
                        filter.Add(Builders<ServiceEntity>.Filter.Eq(t => t.SecondaryId, int.Parse(nodeId.Substring(2))));

            if (!string.IsNullOrEmpty(host) && host != "全部")
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

            var fieldStr = values["Grid1_fields"];
            var arr = string.IsNullOrEmpty(fieldStr)
                ? new JArray("_id", "Name", "ServiceName", "SecondaryName", "Host", "RegContent", "PrimaryId",
                    "SecondaryId", "StartupDate", "IsApproved", "IsAlert", "Version", "Remark")
                : JArray.Parse(fieldStr);
            UpdateGrid(values, filter, arr);
        }

        //
        // GET: /Service/Manage/
        public override ActionResult Index()
        {
            var type = Request["type"];
            var filter = !string.IsNullOrEmpty(type)
                ? t => t.ServiceType == (ServiceTypeEnum) Enum.Parse(typeof(ServiceTypeEnum), type)
                : (Expression<Func<ServiceEntity, bool>>) null;
            _typeFilter = filter;
            var hostList = new List<ListItem>
            {
                new ListItem("全部", "", true)
            };
            hostList.AddRange(Rep.Distinct(t => t.Host, filter).Select(t => new ListItem(t, t)));
            ViewBag.ddlHost = hostList.ToArray();
            ViewBag.TreeNodes = _tree.GetTreeNodes(filter);
            int count;
            var list = Rep.QueryByPage(0, PageSize, out count, filter);
            ViewBag.RecordCount = count;
            ViewBag.PageSize = PageSize;
            return View(list);
        }

        public ActionResult Delete(JArray selectedRows, FormCollection values)
        {
            var ids = selectedRows.Select(Convert.ToInt32).ToList();
            Rep.Delete(t => ids.Contains((int) t._id));
            OnUpdated(values);
            return UIHelper.Result();
        }

    }
}