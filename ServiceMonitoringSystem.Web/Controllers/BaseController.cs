using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using FineUIMvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
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
            int count;
            var list = Rep.QueryByPage(0, PageSize, out count);
            ViewBag.RecordCount = count;
            ViewBag.PageSize = PageSize;
            return View(list);
        }

        public virtual ActionResult DoSearch(FormCollection values)
        {
            OnUpdated(values);
            return UIHelper.Result();
        }
        protected void UpdateGrid(NameValueCollection values, List<FilterDefinition<T>> filter, string gridName = "Grid1")
        {
            var fields = JArray.Parse(values[gridName + "_fields"]);
            var pageIndex = Convert.ToInt32(values[gridName + "_pageIndex"]);
            int count;
            var where = filter != null && filter.Any() ? Builders<T>.Filter.And(filter) : null;
            var list = Rep.QueryByPage(pageIndex, PageSize, out count, where);
            var grid = UIHelper.Grid(gridName);
            grid.RecordCount(count);
            grid.DataSource(list, fields);
        }

    }
}