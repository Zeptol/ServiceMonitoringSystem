using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
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
    public class PhysicalDeviceController : BaseController<PhysicalDevice>
    {
        private readonly ICommon _common;
        //
        // GET: /Service/PhysicalDevice/
        public PhysicalDeviceController(IMongoRepository<PhysicalDevice> rep, ICommon common) : base(rep)
        {
            _common = common;
            Rep = rep;
            Updated+=PhysicalDeviceController_Updated;
        }

        private void PhysicalDeviceController_Updated(NameValueCollection values)
        {
            var name = values["tbxMachineName"];
            var type = values["ddlDeviceType"];
            var owner = values["ddlOwner"];
            var filter = new List<FilterDefinition<PhysicalDevice>>();
            if (!string.IsNullOrEmpty(name))
            {
                filter.Add(Builders<PhysicalDevice>.Filter.Regex(t => t.MachineName,
                    new BsonRegularExpression(new Regex(name, RegexOptions.IgnoreCase))));
            }
            if (!string.IsNullOrEmpty(type) && type != "全部")
            {
                filter.Add(new FilterDefinitionBuilder<PhysicalDevice>().Where(t => t.DeviceType == int.Parse(type)));
            }
            if (!string.IsNullOrEmpty(owner) && owner != "全部")
            {
                filter.Add(new FilterDefinitionBuilder<PhysicalDevice>().Where(t => t.Owner == int.Parse(owner)));
            }
            UpdateGrid(values, filter);
        }

        public override ActionResult Index()
        {
            var typeList = new List<ListItem>
            {
                new ListItem("全部", "", true)
            };
            var ownerList = new List<ListItem>
            {
                new ListItem("全部", "", true)
            };
            typeList.AddRange(_common.GetTypeSelectList("设备类型"));
            ownerList.AddRange(_common.GetTypeSelectList("业主方"));
            ViewBag.ddlDeviceType=typeList.ToArray();
            ViewBag.ddlOwner =ownerList.ToArray();
            return base.Index();
        }

        public ActionResult Delete(JArray selectedRows, FormCollection values)
        {
            Rep.Delete(t => selectedRows.Select(Convert.ToInt32).Contains(t.Rid));
            OnUpdated(values);
            return UIHelper.Result();
        }
        public ActionResult AddOrEdit(int? id)
        {
            ViewBag.DeviceTypeList =_common.GetTypeSelectList("设备类型");
            ViewBag.OwnerList = _common.GetTypeSelectList("业主方");
            if (id==null) return View();
            var model = Rep.Get(t => t.Rid == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddOrEdit(PhysicalDevice model)
        {
            if (!ModelState.IsValid) return UIHelper.Result();
            try
            {
                if (model.Rid==0)
                {
                    if (!CheckRepeat(model))
                    {
                        Alert.Show("已有相同记录存在！", MessageBoxIcon.Warning);
                    }
                    else
                    {
                        model.Rid = (int)(Rep.Max(t => t.Rid) ?? 0) + 1;
                        Rep.Add(model);
                        // 关闭本窗体（触发窗体的关闭事件）
                        PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                    }
                }
                else
                {
                    Rep.Update(t => t.Rid == model.Rid,
                        Builders<PhysicalDevice>.Update.Set(t => t.MachineName, model.MachineName)
                            .Set(t => t.ModelNum, model.ModelNum)
                            .Set(t => t.DeviceType, model.DeviceType)
                            .Set(t => t.Owner, model.Owner)
                            .Set(t => t.PublicIP, model.PublicIP)
                            .Set(t => t.IntranetIP, model.IntranetIP)
                            .Set(t => t.ManagementIP, model.ManagementIP)
                            .Set(t => t.DomainIP, model.DomainIP)
                            .Set(t => t.Cpu, model.Cpu)
                            .Set(t => t.Memory, model.Memory)
                            .Set(t => t.Storage, model.Storage)
                            .Set(t => t.Locale, model.Locale)
                            .Set(t => t.Date, model.Date)
                            .Set(t => t.PurchaseDate, model.PurchaseDate)
                            .Set(t => t.WarrantyExpiry, model.WarrantyExpiry)
                            .Set(t => t.NetTpParam, model.NetTpParam)
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
        private bool CheckRepeat(PhysicalDevice model)
        {
            var modelDb =
                Rep.Get(
                    t =>
                        t.PublicIP == model.PublicIP && t.IntranetIP == model.IntranetIP &&
                        t.MachineName == model.MachineName);
            if (modelDb == null) return true;
            return model.Rid == modelDb.Rid;
        }
    }
}