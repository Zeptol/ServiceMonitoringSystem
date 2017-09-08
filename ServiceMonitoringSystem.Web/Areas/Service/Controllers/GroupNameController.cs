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
using ServiceMonitoringSystem.Web.Controllers;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class GroupNameController : BaseController<GroupName>
    {
        public GroupNameController(IMongoRepository<GroupName> groupName):base(groupName)
        {
            Rep = groupName;
            Updated += GroupNameController_Updated;
        }
        public ViewResult Create()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            var model = Rep.Get(t => t._id== id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        public ActionResult Delete(JArray selectedRows, FormCollection values)
        {
            Rep.Delete(t => selectedRows.Select(Convert.ToInt32).Contains(t._id));
            OnUpdated(values);
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
                            var max = (int) (Rep.Max(t => t._id) ?? 0);
                            model._id = max + 1;
                            model.CreateDateTime = DateTime.Now;
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
                        Rep.Update(t => t._id == model._id,
                            Builders<GroupName>.Update.Set(t => t.ServiceName, model.ServiceName)
                                .Set(t => t.ServiceNameCN, model.ServiceNameCN)
                                .Set(t => t.Remarks, model.Remarks));
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

        private void GroupNameController_Updated(NameValueCollection values)
        {
            var id = values["tbxId"];
            var serName = values["tbxServiceName"];
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
            UpdateGrid(values, filter);
        }
        private bool CheckRepeat(GroupName model)
        {
            var modelDb = Rep.Get(t => t._id == model._id || t.ServiceName == model.ServiceName);
            if (modelDb == null) return true;
            return model._id == modelDb._id;
        }
    }
}