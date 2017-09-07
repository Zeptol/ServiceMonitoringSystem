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
    public class BaseController : Controller
    {
        protected const int PageSize = 20;
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
            Notify n = new Notify();
            n.Target = target;
            n.Message = message;
            n.MessageBoxIcon = messageIcon;
            n.PositionX = Position.Center;
            n.PositionY = Position.Top;
            n.DisplayMilliseconds = 3000;
            n.ShowHeader = false;

            n.Show();
        }

        protected virtual void UpdateGrid<T>(NameValueCollection values, List<FilterDefinition<T>> filter,
            IMongoRepository<T> rep, string gridName = "Grid1") where T : class
        {
            var fields = JArray.Parse(values[gridName + "_fields"]);
            var pageIndex = Convert.ToInt32(values[gridName + "_pageIndex"]);
            int count;
            var where = filter != null && filter.Any() ? Builders<T>.Filter.And(filter) : null;
            var list = rep.QueryByPage(pageIndex, PageSize, out count, where);
            var grid = UIHelper.Grid(gridName);
            grid.RecordCount(count);
            grid.DataSource(list, fields);
        }

    }
}