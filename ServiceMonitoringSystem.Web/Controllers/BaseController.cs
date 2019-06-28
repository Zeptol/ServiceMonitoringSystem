using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using FineUIMvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceMonitoringSystem.Common.Extensions;
using ServiceMonitoringSystem.IRepository;

namespace ServiceMonitoringSystem.Web.Controllers
{
    public class BaseController<T> : Controller where T : class
    {
        protected IMongoRepository<T> Rep;
        protected const int PageSize = 20;

        protected delegate void UpdateGridHandler(NameValueCollection values);

        protected event UpdateGridHandler Updated;
        protected virtual void OnUpdated(NameValueCollection values)
        {
            if (Updated != null) Updated.Invoke(values);
            else UpdateGrid(values, null);
        }
        public BaseController(IMongoRepository<T> rep)
        {
            Rep = rep;
        }
        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public virtual void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            ShowNotify(message, messageIcon, Target.Top);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        /// <param name="target"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        {
            var n = new Notify
            {
                Target = target,
                Message = message,
                MessageBoxIcon = messageIcon,
                PositionX = Position.Center,
                PositionY = Position.Top,
                DisplayMilliseconds = 3000,
                ShowHeader = false
            };

            n.Show();
        }

        public virtual ActionResult Index()
        {
            var list = Rep.QueryByPage(0, PageSize, out var count);
            ViewBag.RecordCount = count;
            ViewBag.PageSize = PageSize;
            return View(list);
        }
        [HttpPost]
        public virtual ActionResult DoSearch(FormCollection values)
        {
            OnUpdated(values);
            return UIHelper.Result();
        }

        protected void UpdateGrid(NameValueCollection values, List<FilterDefinition<T>> filter,JArray fieldsJArray=null,
            string gridName = "Grid1")
        {
            var fieldsStr = values[gridName + "_fields"];
            var fields = fieldsJArray ?? JArray.Parse(fieldsStr);
            var pageIndex = Convert.ToInt32(values[gridName + "_pageIndex"]??"0");
            var sortField = values[gridName + "_sortField"];
            var sortDirection = values[gridName + "_sortDirection"];
            var where = filter != null && filter.Any() ? Builders<T>.Filter.And(filter) : null;
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(sortField))
            {
                if (!string.IsNullOrEmpty(sortDirection))
                {
                    var exp = sortField.ToLambda<T>();
                    if (sortDirection.Equals("ASC", StringComparison.CurrentCultureIgnoreCase))
                        sort = Builders<T>.Sort.Ascending(exp);
                    else if (sortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase))
                        sort = Builders<T>.Sort.Descending(exp);
                }
            }
            var list = Rep.QueryByPage(pageIndex, PageSize, out var count, where, sort);
            // 导出用
            Session["list"] = list;
            var grid = UIHelper.Grid(gridName);
            grid.RecordCount(count);
            grid.DataSource(list, fields);
        }

        #region 上传文件类型判断

        protected readonly List<string> ValidFileTypes = new List<string>
        {
            "zip",
            "rar",
            "tar",
            "7z",
            "jar",
            "cab",
            "iso",
            "ace"
        };

        protected bool ValidateFileType(string fileName)
        {
            var fileType = string.Empty;
            var lastDotIndex = fileName.LastIndexOf(".", StringComparison.Ordinal);
            if (lastDotIndex >= 0)
            {
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();
            }

            return ValidFileTypes.Contains(fileType);
        }


        #endregion


    }
}