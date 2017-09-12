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
    public class AppContactsController : BaseController<AppContacts>
    {
        public AppContactsController(IMongoRepository<AppContacts> rep):base(rep)
        {
            Rep = rep;
            Updated += AppContactsController_Updated;
        }

        private void AppContactsController_Updated(NameValueCollection values)
        {
            var name = values["tbxName"];
            var filter = new List<FilterDefinition<AppContacts>>();
            if (!string.IsNullOrEmpty(name))
                filter.Add(Builders<AppContacts>.Filter.Regex(t => t._id,
                    new BsonRegularExpression(new Regex(name, RegexOptions.IgnoreCase))));
            UpdateGrid(values, filter);
        }

        public ActionResult AddOrEdit(string id)
        {
            if (string.IsNullOrEmpty(id)) return View();
            var model = Rep.Get(t => t._id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public ActionResult Delete(JArray selectedRows, FormCollection values)
        {
            var ids = selectedRows.Select(x => x.ToString()).ToList();
            Rep.Delete(t => ids.Contains(t._id));
            OnUpdated(values);
            return UIHelper.Result();
        }

        [HttpPost]
        public ActionResult AddOrEdit(AppContacts model,bool isAdd=false)
        {
            if (!ModelState.IsValid) return UIHelper.Result();
            try
            {
                if (isAdd)
                {
                    if (!CheckRepeat(model))
                    {
                        Alert.Show("已有相同记录存在！", MessageBoxIcon.Warning);
                    }
                    else
                    {
                        Rep.Add(model);
                        // 关闭本窗体（触发窗体的关闭事件）
                        PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                    }
                }
                else
                {
                    Rep.Update(t => t._id == model._id, Builders<AppContacts>.Update.Set(t => t.Tel, model.Tel)
                        .Set(t => t.Email, model.Email)
                        .Set(t => t.WeiXin_UID, model.WeiXin_UID)
                        .Set(t => t.DingTalk_UID, model.DingTalk_UID)
                        .Set(t => t.AlarmType, model.AlarmType)
                        .Set(t => t.Remark, model.Remark));
                    // 关闭本窗体（触发窗体的关闭事件）
                    PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, MessageBoxIcon.Warning);
            }
            return UIHelper.Result();
        }
        private bool CheckRepeat(AppContacts model)
        {
            var modelDb = Rep.Get(t => t._id == model._id);
            return modelDb==null;
        }
    }
}