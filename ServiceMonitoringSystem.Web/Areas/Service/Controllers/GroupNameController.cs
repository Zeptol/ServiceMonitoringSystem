using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using FineUIMvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
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
            var list = _groupName.QueryByPage(0, PageSize, out count);
            ViewBag.RecordCount = count;
            ViewBag.PageSize = PageSize;
            return View(list);
        }

        public AjaxResult DoSearch(FormCollection values)
        {
           UpdateGrid(values);
            return UIHelper.Result();
        }
        public ViewResult Create()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            var model = _groupName.Get(t => t._id== id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        public ActionResult Delete(JArray selectedRows, FormCollection values)
        {
            _groupName.Delete(t => selectedRows.Select(Convert.ToInt32).Contains(t._id));
            UpdateGrid(values);
            return UIHelper.Result();
        }

        public ActionResult btnCreate_Click(GroupName model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.ServiceName))
                    {
                        if (!CheckRepeat(model))
                        {
                            Alert.Show("已有相同记录存在！", MessageBoxIcon.Warning);
                        }
                        else
                        {
                            var max = (int) (_groupName.Max(t => t._id) ?? 0);
                            model._id = max + 1;
                            model.CreateDateTime = DateTime.Now;
                            _groupName.Add(model);
                        }
                    }
                    else
                    {
                        Alert.Show("名称不能为空！", MessageBoxIcon.Warning);
                    }
                    // 关闭本窗体（触发窗体的关闭事件）
                    PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                }
                catch (Exception ex)
                {
                    Alert.Show(ex.Message, MessageBoxIcon.Warning);
                }
            }
            return UIHelper.Result();
        }

        public ActionResult btnEdit_Click(GroupName model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!CheckRepeat(model))
                    {
                        Alert.Show("已有相同记录存在！", MessageBoxIcon.Warning);
                    }
                    else
                    {
                        _groupName.Update(t => t._id == model._id,
                            Builders<GroupName>.Update.Set(t => t.ServiceName, model.ServiceName)
                                .Set(t => t.ServiceNameCN, model.ServiceNameCN)
                                .Set(t => t.Remarks, model.Remarks));
                    }
                    // 关闭本窗体（触发窗体的关闭事件）
                    PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                }
                catch (Exception ex)
                {
                    Alert.Show(ex.Message, MessageBoxIcon.Warning);
                }
            }
            return UIHelper.Result();
        }

        private void UpdateGrid(NameValueCollection values)
        {
            var id = values["tbxId"];
            var serName = values["tbxServiceName"];
            var fields = JArray.Parse(values["Grid1_fields"]);
            var pageIndex = Convert.ToInt32(values["Grid1_pageIndex"]);
            int count;
            var filter = new List<FilterDefinition<GroupName>>();
            if (!string.IsNullOrEmpty(id))
            {
                int idInt;
                int.TryParse(id, out idInt);
                filter.Add(Builders<GroupName>.Filter.Eq(t => t._id, idInt));
            }

            if (!string.IsNullOrEmpty(serName))
            {
                filter.Add(
                    Builders<GroupName>.Filter.Or(
                        Builders<GroupName>.Filter.Regex(t => t.ServiceName,
                            new BsonRegularExpression(new Regex(serName, RegexOptions.IgnoreCase))),
                        Builders<GroupName>.Filter.Regex(t => t.ServiceNameCN,
                            new BsonRegularExpression(new Regex(serName, RegexOptions.IgnoreCase)))));
            }
            var where = filter.Any() ? Builders<GroupName>.Filter.And(filter) : null;
            var list = _groupName.QueryByPage(pageIndex, PageSize, out count, where);
            var grid = UIHelper.Grid("Grid1");
            grid.RecordCount(count);
            grid.DataSource(list, fields);
        }
        private bool CheckRepeat(GroupName model)
        {
            var modelDb = _groupName.Get(t => t._id == model._id || t.ServiceName == model.ServiceName);
            if (modelDb == null) return true;
            return model._id == modelDb._id;
        }
    }
}