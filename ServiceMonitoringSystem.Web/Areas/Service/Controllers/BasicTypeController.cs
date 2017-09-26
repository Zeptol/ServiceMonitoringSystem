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
using ServiceMonitoringSystem.Interface;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;
using ServiceMonitoringSystem.Web.Controllers;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class BasicTypeController : BaseController<BasicType>
    {
        private readonly ICommon _common;
        public BasicTypeController(ICommon common, IMongoRepository<BasicType> rep)
            : base(rep)
        {
            _common = common;
            Rep = rep;
            Updated += BasicTypeController_Updated;
        } 
        //
        // GET: /Service/BasicType/
        public override ActionResult Index()
        {
            var typeList = _common.GetTypeList().ToList();
            typeList.Insert(0, new ListItem
            {
                Text = "全部",
                Value = string.Empty
            });
            ViewBag.TypeList = typeList.ToArray();
            return base.Index();
        }

        private void BasicTypeController_Updated(NameValueCollection values)
        {
            var type = values["ddlType"];
            var num = values["tbxNum"];
            var name = values["tbxName"];
            var filter = new List<FilterDefinition<BasicType>>();
            if (!string.IsNullOrEmpty(type))
            {
                int typeInt;
                int.TryParse(type, out typeInt);
                filter.Add(Builders<BasicType>.Filter.Eq(t => t.TypeId, typeInt));
            }
            if (!string.IsNullOrEmpty(num))
            {
                int numInt;
                int.TryParse(type, out numInt);
                filter.Add(Builders<BasicType>.Filter.Eq(t => t.Num, numInt));
            }
            if (!string.IsNullOrEmpty(name))
            {
                filter.Add(Builders<BasicType>.Filter.Regex(t => t.Name,
                    new BsonRegularExpression(new Regex(name, RegexOptions.IgnoreCase))));
            }
            UpdateGrid(values, filter);
        }
        public ViewResult Create()
        {
            ViewBag.TypeList = _common.GetTypeList();
            return View();
        }
        public ActionResult Edit(int id)
        {
            var model = Rep.Get(t => t.Rid == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeList = _common.GetTypeList();
            return View(model);
        }
        public ActionResult Delete(JArray selectedRows, FormCollection values)
        {
            Rep.Delete(t => selectedRows.Select(Convert.ToInt32).Contains(t.Rid));
            OnUpdated(values);
            return UIHelper.Result();
        }

        public ActionResult btnCreate_Click(BasicType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.Name))
                    {
                        if (!CheckRepeat(model))
                        {
                            Alert.Show("已有相同记录存在！", MessageBoxIcon.Warning);
                        }
                        else
                        {
                            var max = (int)(Rep.Max(t => t.Rid) ?? 0);
                            model.Rid = max + 1;
                            Rep.Add(model);
                            // 关闭本窗体（触发窗体的关闭事件）
                            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                        }
                    }
                    else
                    {
                        Alert.Show("名称不能为空！", MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Alert.Show(ex.Message, MessageBoxIcon.Warning);
                }
            }
            return UIHelper.Result();
        }

        public ActionResult btnEdit_Click(BasicType model)
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
                        Rep.Update(t => t.Rid == model.Rid,
                            Builders<BasicType>.Update.Set(t => t.TypeId, model.TypeId)
                                .Set(t => t.Num, model.Num)
                                .Set(t => t.Name, model.Name));
                        // 关闭本窗体（触发窗体的关闭事件）
                        PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                    }
                }
                catch (Exception ex)
                {
                    Alert.Show(ex.Message, MessageBoxIcon.Warning);
                }
            }
            return UIHelper.Result();
        }
        private bool CheckRepeat(BasicType model)
        {
            var modelDb = Rep.Get(t => t.Num == model.Num || t.Name == model.Name);
            if (modelDb == null) return true;
            return model.Rid == modelDb.Rid;
        }
    }
}