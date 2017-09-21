using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FineUIMvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceMonitoringSystem.Common.Common;
using ServiceMonitoringSystem.Common.Enums;
using ServiceMonitoringSystem.Common.Extensions;
using ServiceMonitoringSystem.Interface;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;
using ServiceMonitoringSystem.Web.Controllers;

namespace ServiceMonitoringSystem.Web.Areas.Service.Controllers
{
    public class ManageController : BaseController<ServiceEntity>
    {
        private readonly ITree _tree;
        private static Expression<Func<ServiceEntity,bool>> _typeFilter;
        private readonly IMongoRepository<GroupName> _group;
        private readonly IMongoRepository<FileEntity> _file;
        private readonly IMongoRepository<ServiceAlertContacts> _alertContacts;
        public ManageController(IMongoRepository<ServiceEntity> rep, IMongoRepository<GroupName> group,ITree tree, IMongoRepository<FileEntity> file, IMongoRepository<ServiceAlertContacts> alertContacts) : base(rep)
        {
            _tree = tree;
            _file = file;
            _alertContacts = alertContacts;
            _group = group;
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
        public ActionResult AddOrEdit(int? id)
        {
            ViewBag.ServiceList = _group.Find().ToList().Select(t => new ListItem
            {
                Text = t.ServiceNameCN,
                Value = t.ServiceNameCN
            }).ToArray();
            if (id==null) return View();
            var model = Rep.Get(t => t._id == id);
            if (model == null)
            {
                return HttpNotFound();
            } 
            if (!string.IsNullOrWhiteSpace(model.RegContent))
            {
                InitAddr(model);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddOrEdit(ServiceEntity model)
        {
            if (!ModelState.IsValid) return UIHelper.Result();
            try
            {
                if (model._id==0)
                {
                    if (!CheckRepeat(model))
                    {
                        Alert.Show("已有相同记录存在！", MessageBoxIcon.Warning);
                    }
                    else
                    {
                        model._id = (int)(Rep.Max(t => t._id) ?? 0) + 1;
                        model.PrimaryId = (int)(Rep.Max(t => t.PrimaryId) ?? 0) + 1;
                        model.SecondaryId = (int)(Rep.Max(t => t.SecondaryId) ?? 0) + 1;
                        SetRegContent(model);
                        Rep.Add(model);
                        // 关闭本窗体（触发窗体的关闭事件）
                        PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                    }
                }
                else
                {
                    SetRegContent(model);
                    Rep.Update(t => t._id == Convert.ToInt32(model._id), Builders<ServiceEntity>.Update.Set(t => t.ServiceName, model.ServiceName)
                        .Set(t => t.SecondaryName, model.SecondaryName)
                        .Set(t => t.Host, model.Host)
                        .Set(t => t.Version, model.Version)
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
        public ActionResult Delete(JArray selectedRows, FormCollection values)
        {
            var ids = selectedRows.Select(Convert.ToInt32).ToList();
            Rep.Delete(t => ids.Contains(t._id));
            OnUpdated(values);
            return UIHelper.Result();
        }

        public ActionResult Detail(int id)
        {
            var model = Rep.Get(t => t._id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            int count;
            ViewBag.FileList = _file.QueryByPage(0, PageSize, out count,
                new FilterDefinitionBuilder<FileEntity>().Where(t => t.SecondaryId == model.SecondaryId));
            int countContacts;
            ViewBag.AlertContactsList = _alertContacts.QueryByPage(0, PageSize, out countContacts,
                new FilterDefinitionBuilder<ServiceAlertContacts>().Where(t => t.ServiceId == model._id));
            ViewBag.RecordCountContacts = countContacts;
            ViewBag.RecordCountFiles = count;
            ViewBag.PageSize = PageSize;
            ViewBag.ServiceId = model._id;
            ViewBag.PrimaryId = model.PrimaryId;
            ViewBag.SecondaryId = model.SecondaryId;
            if (!string.IsNullOrWhiteSpace(model.RegContent))
                InitAddr(model);
            return View(model);
        }
        private void InitAddr(ServiceEntity model)
        {
            var cfg = JsonConvert.DeserializeObject<ServiceConf>(model.RegContent);
            ViewBag.InList = cfg.InAddr;
            ViewBag.OutList = cfg.OutAddr;
        }
        private void SetRegContent(ServiceEntity model)
        {
            var inList = new List<string>();
            var outList = new List<string>();
            var inPorts = new List<string>();
            var outPorts = new List<string>();
            var inStr = Request["inAddr"];
            if (!string.IsNullOrWhiteSpace(inStr))
            {
                inList = inStr.Split(',').ToList();
            }
            var outStr = Request["outAddr"];
            if (!string.IsNullOrWhiteSpace(outStr))
            {
                outList = outStr.Split(',').ToList();
            }
            var inPortStr = Request["inPort"];
            if (!string.IsNullOrWhiteSpace(inStr))
            {
                inPorts = inPortStr.Split(',').ToList();
            }
            var outPortStr = Request["outPort"];
            if (!string.IsNullOrWhiteSpace(inStr))
            {
                outPorts = outPortStr.Split(',').ToList();
            }
            var cfg = new ServiceConf
            {
                InAddr = inList.Zip(inPorts, (t, p) => !string.IsNullOrEmpty(p) ? t + ":" + p : t).ToList(),
                OutAddr = outList.Zip(outPorts, (t, p) => !string.IsNullOrEmpty(p) ? t + ":" + p : t).ToList(),
                Remarks = model.Remark
            };
            model.RegContent = JsonConvert.SerializeObject(cfg);
        }
        private bool CheckRepeat(ServiceEntity model)
        {
            var modelDb = Rep.Get(t => t.ServiceName == model.ServiceName && t.SecondaryName == model.SecondaryName);
            if (modelDb == null) return true;
            return model._id == modelDb._id;
        }

        public ActionResult ExportToExcel()
        {
            int count;
            var list =
                ((List<ServiceEntity>) Session["list"] ?? Rep.QueryByPage(0, int.MaxValue, out count)).Where(
                    _typeFilter.Compile()).ToList();
            const string thHtml = "<th>{0}</th>";
            const string tdHtml = "<td>{0}</td>";

            var sb = new StringBuilder();
            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");
            sb.Append("<tr>");
            sb.AppendFormat(thHtml, "");
            sb.AppendFormat(thHtml, "服务ID");
            sb.AppendFormat(thHtml, "二级服务ID");
            sb.AppendFormat(thHtml, "内网IP");
            sb.AppendFormat(thHtml, "内网端口");
            sb.AppendFormat(thHtml, "外网IP");
            sb.AppendFormat(thHtml, "外网端口");
            sb.AppendFormat(thHtml, "服务名称");
            sb.AppendFormat(thHtml, "二级服务名称");
            sb.AppendFormat(thHtml, "版本");
            sb.AppendFormat(thHtml, "备注");
            sb.Append("</tr>");

            var rowIndex = 1;
            foreach (var item in list)
            {
                sb.Append("<tr>");
                sb.AppendFormat(tdHtml, rowIndex++);
                sb.AppendFormat(tdHtml, item.PrimaryId);
                sb.AppendFormat(tdHtml, item.SecondaryId);

                var conf = new ServiceConf();
                if (!string.IsNullOrWhiteSpace(item.RegContent))
                    conf = JsonConvert.DeserializeObject<ServiceConf>(item.RegContent);
                var inList = conf.InAddr ?? new List<string>();
                var outList = conf.OutAddr ?? new List<string>();

                var inFlag = inList.FirstOrDefault(t => t.Contains(item.Host)) != null;
                var inAddr = inFlag
                    ? inList.First(t => t.Contains(item.Host))
                        .Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                    : new string[0];
                sb.AppendFormat(tdHtml, item.Host);

                sb.AppendFormat(tdHtml, inFlag ? inAddr.Length > 1 ? inAddr[1] : "" : "");
                var outAddr = new List<string>();
                var outPort = new List<string>();
                if (outList.Any())
                {
                    var @out = outList.Select(t =>
                    {
                        var arr = t.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        return new { Addr = arr[0], Port = arr.Length > 1 ? arr[1] : "" };
                    }).ToList();
                    outAddr = @out.Select(t => t.Addr).ToList();
                    outPort = @out.Select(t => t.Port).ToList();
                }
                sb.AppendFormat(tdHtml, string.Join(",", outAddr));
                sb.AppendFormat(tdHtml, string.Join(",", outPort));

                sb.AppendFormat(tdHtml, item.ServiceName);
                sb.AppendFormat(tdHtml, item.SecondaryName);
                sb.AppendFormat(tdHtml, item.Version);
                sb.AppendFormat(tdHtml, item.Remark);
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/excel", "服务列表.xls");
        }

        public ActionResult DeleteFile(JArray selectedRows,FormCollection values)
        {
            var ids = selectedRows.Select(Convert.ToInt32).ToList();
            var files = _file.Find(t => ids.Contains(t.Rid)).ToList();
            _file.Delete(t => ids.Contains(t.Rid));
            RefreshFileList(values);
            foreach (var file in files)
            {
                if (System.IO.File.Exists(file.Url))
                {
                    System.IO.File.Delete(file.Url);
                }
            }
            return UIHelper.Result();
        }
        public ActionResult UploadFile(int sec)
        {
            ViewBag.SecondaryId = sec;
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file == null|| file.ContentLength==0) return UIHelper.Result();
            var fileName = file.FileName;
            if (!ValidateFileType(fileName))
            {
                // 清空文件上传组件
                UIHelper.FileUpload("file").Reset();
                ShowNotify("无效的文件类型！");
            }
            else
            {
                var size = file.ContentLength;
                var md5 = CommonHelper.GetMd5(file.InputStream);
                var model = _file.Get(t => t.Md5 == md5);
                if (model != null)
                {
                    ShowNotify("文件已存在");
                    return UIHelper.Result();
                }
                var path = Server.MapPath("~/upload/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var filename = Path.GetFileName(file.FileName);
                if (filename == null)
                {
                    ShowNotify("空文件名");
                    return UIHelper.Result();
                }

                var extStart = filename.LastIndexOf(".", StringComparison.Ordinal);
                var ext = filename.Substring(extStart);
                var newName = string.Format("{0}_{1}{2}", filename.Substring(0, extStart), Guid.NewGuid(), ext);

                try
                {
                    var url = Path.Combine(path, filename);
                    var entity = new FileEntity
                    {
                        Author = "",
                        DateTime = DateTime.Now,
                        FileName = filename,
                        Md5 = md5,
                        Size = size,
                        SecondaryId = int.Parse(Request["SecondaryId"])
                    };
                    if (System.IO.File.Exists(url))
                    {
                        url = Path.Combine(path, newName);
                        entity.FileName = newName;
                    }
                    file.SaveAs(url);
                    entity.Url = url;
                    entity.Rid = (int)(_file.Max(t => t.Rid) ?? 0) + 1;
                    _file.Add(entity);
                }
                catch (Exception ex)
                {
                    ShowNotify(ex.Message, MessageBoxIcon.Warning);
                    return UIHelper.Result();
                }
                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }
        public ActionResult DownloadFile(int id)
        {
            var file = _file.Get(t => t.Rid == id);
            if (file == null) return HttpNotFound();
            return File(file.Url, "application/octet-stream", file.FileName);
        }

        public ActionResult RefreshFileList(FormCollection values)
        {
            var fieldsStr = values["GridFiles_fields"];
            var fields = JArray.Parse(fieldsStr);
            var pageIndex = Convert.ToInt32(values["GridFiles_pageIndex"] ?? "0");
            int count;
            var list = _file.QueryByPage(pageIndex, PageSize, out count, new FilterDefinitionBuilder<FileEntity>().Where(t => t.SecondaryId == int.Parse(Request["SecondaryId"])));
            var grid = UIHelper.Grid("GridFiles");
            grid.RecordCount(count);
            grid.DataSource(list, fields);
            return UIHelper.Result();
        }
        public ActionResult RefreshAlertContactsList(FormCollection values)
        {
            var fieldsStr = values["GridAlertContacts_fields"];
            var fields = JArray.Parse(fieldsStr);
            var pageIndex = Convert.ToInt32(values["GridAlertContacts_pageIndex"] ?? "0");
            int count;
            var list = _alertContacts.QueryByPage(pageIndex, PageSize, out count, new FilterDefinitionBuilder<ServiceAlertContacts>().Where(t => t.ServiceId == int.Parse(Request["id"])));
            var grid = UIHelper.Grid("GridAlertContacts");
            grid.RecordCount(count);
            grid.DataSource(list, fields);
            return UIHelper.Result();
        }

        public ActionResult DeleteAlertContacts(JArray selectedRows, FormCollection values)
        {
            var ids = selectedRows.Select(x => x.ToString()).ToList();
            _alertContacts.Delete(t => ids.Contains(t._id));
            RefreshAlertContactsList(values);
            return UIHelper.Result();
        }
        public ActionResult AddOrEditContacts(string id)
        {
            ViewBag.ServiceId = Request["ServiceId"];
            ViewBag.PrimaryId = Request["PrimaryId"];
            if (string.IsNullOrEmpty(id)) return View();
            var model = _alertContacts.Get(t => t._id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddOrEditContacts(ServiceAlertContacts model)
        {
            if (!ModelState.IsValid) return UIHelper.Result();
            try
            {
                if (string.IsNullOrEmpty(model._id))
                {
                    var serviceId = Request["ServiceId"];
                    var primaryId = Request["PrimaryId"];
                    model.ServiceId = int.Parse(serviceId);
                    model.PrimaryId = int.Parse(primaryId);
                        _alertContacts.Add(model);
                        // 关闭本窗体（触发窗体的关闭事件）
                        PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
                }
                else
                {
                    _alertContacts.Update(t => t._id == model._id, Builders<ServiceAlertContacts>.Update.Set(t=>t.UserName,model.UserName).Set(t => t.Tel, model.Tel)
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

        public ActionResult SyncService(string id)
        {
            int idInt;
            if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out idInt))
            {
                ShowNotify("无效同步服务ID");
                return UIHelper.Result();
            }
            try
            {
                var list = _alertContacts.Find(t => t.ServiceId == idInt).ToList();
                if (list.Count > 0)
                {
                    var model = Rep.Get(t => t._id == idInt);
                    var sList = Rep.Find(t => t.PrimaryId == model.PrimaryId && t._id != idInt).ToList();

                    var rList = new List<ServiceAlertContacts>();

                    sList.ForEach(k =>
                    {
                        list.ForEach(l =>
                        {
                            var m = (ServiceAlertContacts) l.Clone();
                            m._id = Guid.NewGuid().ToString();
                            m.ServiceId = Convert.ToInt32(k._id);
                            m.PrimaryId = k.PrimaryId;
                            rList.Add(m);
                        });
                    });
                    _alertContacts.Delete(t => t.PrimaryId == model.PrimaryId && t.ServiceId != idInt);
                    _alertContacts.BulkInsert(rList);
                    ShowNotify("同步成功");
                }
                else
                {
                    ShowNotify("无同步数据");
                }
            }
            catch (Exception e)
            {
                Alert.Show(e.Message, MessageBoxIcon.Warning);
                throw;
            }
            return UIHelper.Result();
        }
    }
}