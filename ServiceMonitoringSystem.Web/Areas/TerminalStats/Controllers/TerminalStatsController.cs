using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FineUIMvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceMonitoringSystem.Common.Enums;
using ServiceMonitoringSystem.IRepository;
using ServiceMonitoringSystem.Model;

namespace ServiceMonitoringSystem.Web.Areas.TerminalStats.Controllers
{
    public class TerminalStatsController : Controller
    {
        private readonly IMongoRepositoryBusData<tblVehEquipment> _repEquipment;
        private readonly IMongoRepositoryBusData<tblVehRoute> _repRoute;
        private const int PageSize = 20;
        public TerminalStatsController(IMongoRepositoryBusData<tblVehRoute> repRoute, IMongoRepositoryBusData<tblVehEquipment> repEquipment)
        {
            _repRoute = repRoute;
            _repEquipment = repEquipment;
        }

        //
        // GET: /TerminalStats/TerminalStats/
        public ActionResult Index()
        {
            //var filter = Builders<tblGPS>.Filter.Gte(t => t.GPSDateTime,
            //    DateTime.Today.AddDays(-3).ToString(CultureInfo.CurrentCulture));
            //var vids =
            //    _repGps.Find(filter).ToList().Select(t => t._id).ToList();

            var eqp = _repEquipment.Find().ToList();
            var ids = eqp.Select(t => t._id).Distinct().ToList();
            var routes = _repRoute.Find(t => ids.Contains(t._id)).ToList();

            var list = (from eq in eqp
                        join r in routes on eq._id equals r._id into g
                        from gg in g.DefaultIfEmpty()
                        select
                        new
                        {
                            eq._id,
                            eq.LineName,
                            eq.Manufacturer,
                            eq.SoftVersion,
                            IPAddress_F = gg != null ? gg.IPAddress_F : null,
                            IPAddress_S = gg != null ? gg.IPAddress_S : null,
                            IPAddress_T = gg != null ? gg.IPAddress_T : null
                        }).ToList();

            ViewBag.RecordCount = list.Count;
            ViewBag.PageSize = PageSize;
            return View(list.Take(PageSize));
        }

        public ViewResult Stats()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetHtml()
        {
            var eqp = _repEquipment.Find().ToList();
            var ids = eqp.Select(t => t._id).Distinct().ToList();
            var routes = _repRoute.Find(t => ids.Contains(t._id)).ToList();
            var list = new List<dynamic>((from eq in eqp
                join r in routes on eq._id equals r._id 
                select
                new
                {
                    eq._id,
                    eq.LineName,
                    eq.Manufacturer,
                    eq.SoftVersion,
                    r.IPAddress_F,
                    r.IPAddress_S,
                    r.IPAddress_T
                }).ToList());
            return Json(string.Join("<br/><br/>", GetFirstTable(list), GetSecondTable(list), GetThirdTable(list)));
        }

        private string GetFirstTable(List<dynamic> list)
        {
            var htmlStr =
                new StringBuilder(
                    "<table class=\"x-table\" style=\"left: 0; table-layout: fixed; width: 857px;text-align:center\" cellspacing=\"0\" cellpadding=\"0\"><colgroup><col style=\"width: 50px;\"><col style=\"width: 150px;\"><col style=\"width: 60px;\"><col style=\"width: 150px;\"><col style=\"width: 60px;\"></colgroup><tbody><tr style=\"height: 29px;\"><td class=\"td colName\" style=\"text-align: center;\">序号</td><td class=\"td colName\" style=\"text-align: center;\">一道地址</td><td class=\"td colName\" style=\"text-align: center;\">对应终端数量</td><td class=\"td colName\" style=\"text-align: center;\">终端地址</td><td class=\"td colName\" style=\"text-align: center;\">对应车辆数</td></tr>");
            var fs = list.Select(t => t.IPAddress_F).Distinct().ToList();
            var no = 0;
            foreach (var f in fs)
            {
                no++;
                var ts =
                    list.Where(t => t.IPAddress_F == f)
                        .GroupBy(t => t.IPAddress_T)
                        .Select(t => new { t.Key, Count = t.Count() })
                        .ToList();
                var tf = ts.First();
                if (ts.Count > 1)
                {
                    htmlStr.AppendFormat(
                        "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\" rowspan=\"{5}\">{0}</td><td class=\"td td-data azure\" rowspan=\"{5}\">{1}</td><td class=\"td td-data azure\" rowspan=\"{5}\">{2}</td><td class=\"td td-data\">{3}</td><td class=\"td td-data\">{4}</td></tr>",
                        no, f, ts.Count, tf.Key, tf.Count, ts.Count);
                    ts.RemoveAt(0);
                    foreach (var t in ts)
                    {
                        htmlStr.AppendFormat(
                            "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\">{0}</td><td class=\"td td-data\">{1}</td></tr>",
                            t.Key, t.Count);
                    }
                }
                else
                {
                    htmlStr.AppendFormat(
                        "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\">{0}</td><td class=\"td td-data azure\">{1}</td><td class=\"td td-data azure\">{2}</td><td class=\"td td-data\">{3}</td><td class=\"td td-data\">{4}</td></tr>",
                        no, f, ts.Count, tf.Key, tf.Count);
                }
            }

            htmlStr.Append("</tbody></table>");
            return htmlStr.ToString();
        }

        private string GetSecondTable(List<dynamic> list)
        {
            var htmlStr =
                new StringBuilder(
                    "<table class=\"x-table\" style=\"left: 0; table-layout: fixed; width: 857px;text-align:center\" cellspacing=\"0\" cellpadding=\"0\"><colgroup><col style=\"width: 50px;\"><col style=\"width: 150px;\"><col style=\"width: 150px;\"><col style=\"width: 60px;\"><col style=\"width: 60px;\"><col style=\"width: 60px;\"></colgroup><tbody><tr style=\"height: 29px;\"><td class=\"td colName\" style=\"text-align: center;\">序号</td><td class=\"td colName\" style=\"text-align: center;\">终端地址</td><td class=\"td colName\" style=\"text-align: center;\">一道地址</td><td class=\"td colName\" style=\"text-align: center;\">对应厂商数量</td><td class=\"td colName\" style=\"text-align: center;\">厂商</td><td class=\"td colName\" style=\"text-align: center;\">对应车辆数</td></tr>");
            var tfs = list.GroupBy(t => new { t.IPAddress_T, t.IPAddress_F }).ToList();
            var no = 0;
            foreach (var tf in tfs)
            {
                no++;
                var ms =
                    list.Where(t => t.IPAddress_F == tf.Key.IPAddress_F && t.IPAddress_T == tf.Key.IPAddress_T)
                        .GroupBy(t => t.Manufacturer)
                        .Select(t => new {t.Key, Count = t.Count()})
                        .ToList();
                var mf = ms.First();
                if (ms.Count > 1)
                {
                    htmlStr.AppendFormat(
                        "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\" rowspan=\"{6}\">{0}</td><td class=\"td td-data azure\" rowspan=\"{6}\">{1}</td><td class=\"td td-data azure\" rowspan=\"{6}\">{2}</td><td class=\"td td-data \" rowspan=\"{6}\">{3}</td><td class=\"td td-data\">{4}</td><td class=\"td td-data\">{5}</td></tr>",
                        no, tf.Key.IPAddress_T, tf.Key.IPAddress_F, ms.Count, !string.IsNullOrWhiteSpace(mf.Key) ? ((Manufacturer)char.Parse(mf.Key)).ToString() : "", mf.Count, ms.Count);
                    ms.RemoveAt(0);
                    foreach (var t in ms)
                    {
                        htmlStr.AppendFormat(
                            "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\">{0}</td><td class=\"td td-data\">{1}</td></tr>",
                           !string.IsNullOrWhiteSpace(t.Key) ? ((Manufacturer)char.Parse(t.Key)).ToString() : "", t.Count);
                    }
                }
                else
                {
                    htmlStr.AppendFormat(
                        "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\">{0}</td><td class=\"td td-data azure\">{1}</td><td class=\"td td-data azure\">{2}</td><td class=\"td td-data\">{3}</td><td class=\"td td-data\">{4}</td><td class=\"td td-data\">{5}</td></tr>",
                         no, tf.Key.IPAddress_T, tf.Key.IPAddress_F, ms.Count, !string.IsNullOrWhiteSpace(mf.Key) ? ((Manufacturer)char.Parse(mf.Key)).ToString() : "", mf.Count);
                }
            }

            htmlStr.Append("</tbody></table>");
            return htmlStr.ToString();
        }
        private string GetThirdTable(List<dynamic> list)
        {
            var htmlStr =
                new StringBuilder(
                    "<table class=\"x-table\" style=\"left: 0; table-layout: fixed; width: 857px;text-align:center\" cellspacing=\"0\" cellpadding=\"0\"><colgroup><col style=\"width: 50px;\"><col style=\"width: 150px;\"><col style=\"width: 60px;\"><col style=\"width: 150px;\"><col style=\"width: 150px;\"><col style=\"width: 60px;\"></colgroup><tbody><tr style=\"height: 29px;\"><td class=\"td colName\" style=\"text-align: center;\">序号</td><td class=\"td colName\" style=\"text-align: center;\">厂商</td><td class=\"td colName\" style=\"text-align: center;\">对应终端数量</td><td class=\"td colName\" style=\"text-align: center;\">终端地址</td><td class=\"td colName\" style=\"text-align: center;\">一道地址</td><td class=\"td colName\" style=\"text-align: center;\">车辆数</td></tr>");
            var ms = list.Select(t => t.Manufacturer).Distinct().OrderBy(t => t).ToList();
            var no = 0;
            foreach (var m in ms)
            {
                no++;
                var ts =
                    list.Where(t => t.Manufacturer == m)
                        .GroupBy(t => new {t.IPAddress_T, t.IPAddress_F})
                        .Select(t => new {t.Key, Count = t.Count()})
                        .ToList();
                var tf = ts.First();
                if (ts.Count > 1)
                {
                    htmlStr.AppendFormat(
                        "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\" rowspan=\"{6}\">{0}</td><td class=\"td td-data azure\" rowspan=\"{6}\">{1}</td><td class=\"td td-data azure\" rowspan=\"{6}\">{2}</td><td class=\"td td-data\">{3}</td><td class=\"td td-data\">{4}</td><td class=\"td td-data\">{5}</td></tr>",
                        no, !string.IsNullOrWhiteSpace(m) ? ((Manufacturer)char.Parse(m)).ToString() : "", ts.Count, tf.Key.IPAddress_T, tf.Key.IPAddress_F, tf.Count, ts.Count);
                    ts.RemoveAt(0);
                    foreach (var t in ts)
                    {
                        htmlStr.AppendFormat(
                            "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\">{0}</td><td class=\"td td-data\">{1}</td><td class=\"td td-data\">{2}</td></tr>",
                            t.Key.IPAddress_T,t.Key.IPAddress_F, t.Count);
                    }
                }
                else
                {
                    htmlStr.AppendFormat(
                        "<tr class=\"tr\" style=\"text-align: center;\"><td class=\"td td-data\">{0}</td><td class=\"td td-data azure\">{1}</td><td class=\"td td-data azure\">{2}</td><td class=\"td td-data\">{3}</td><td class=\"td td-data\">{4}</td><td class=\"td td-data\">{5}</td></tr>",
                         no, !string.IsNullOrWhiteSpace(m) ? ((Manufacturer)char.Parse(m)).ToString() : "", ts.Count, tf.Key.IPAddress_T, tf.Key.IPAddress_F, tf.Count);
                }
            }

            htmlStr.Append("</tbody></table>");
            return htmlStr.ToString();
        }


        private void UpdateGrid(NameValueCollection values)
        {
            var fields = JArray.Parse(values["Grid1_fields"]);
            var pageIndex = Convert.ToInt32(values["Grid1_pageIndex"] ?? "0");
            var pageSize = Convert.ToInt32(values["Grid1_pageSize"] ?? "0");

            var tbVehNum = values["tbVehNum"];
            var tbLine = values["tbLine"];
            var ddlFac = values["ddlFac"];
            var tbIpf = values["tbIpf"];
            var tbIps = values["tbIps"];
            var tbIpt = values["tbIpt"];

            //var vids =
            //    _repGps.Find(
            //        t =>
            //            string.Compare(t.GPSDateTime, DateTime.Today.AddDays(-3).ToString(CultureInfo.CurrentCulture),
            //                StringComparison.Ordinal) > 0).ToList().Select(t => t._id).ToList();
            var filter = new List<FilterDefinition<tblVehEquipment>>();
            if (!string.IsNullOrWhiteSpace(tbVehNum))
            {
                filter.Add(Builders<tblVehEquipment>.Filter.Eq(t => t._id, tbVehNum));
            }
            if (!string.IsNullOrWhiteSpace(tbLine))
            {
                filter.Add(Builders<tblVehEquipment>.Filter.Eq(t => t.LineName, tbLine));
            }
            if (!string.IsNullOrWhiteSpace(ddlFac))
            {
                filter.Add(Builders<tblVehEquipment>.Filter.Eq(t => t.Manufacturer, ddlFac));
            }

            var eqp = _repEquipment.Find(filter.Any() ? Builders<tblVehEquipment>.Filter.And(filter) : null).ToList();
            var grid1 = UIHelper.Grid("Grid1");
            var ids = eqp.Select(t => t._id).Distinct().ToList();
            var routes = _repRoute.Find(t => ids.Contains(t._id)).ToList();

            var flag = false;
            if (!string.IsNullOrWhiteSpace(tbIpf))
            {
                routes = routes.Where(t => t.IPAddress_F == tbIpf).ToList();
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(tbIps))
            {
                routes = routes.Where(t => t.IPAddress_S == tbIps).ToList();
                flag = true;
            }
            if (!string.IsNullOrWhiteSpace(tbIpt))
            {
                routes = routes.Where(t => t.IPAddress_T == tbIpt).ToList();
                flag = true;
            }
            if (flag)
            {
                eqp = eqp.Where(t => routes.Select(x => x._id).Contains(t._id)).ToList();
            }

            var list = (from eq in eqp
                        join r in routes on eq._id equals r._id into g
                        from gg in g.DefaultIfEmpty()
                        select
                        new
                        {
                            eq._id,
                            eq.LineName,
                            eq.Manufacturer,
                            eq.SoftVersion,
                            IPAddress_F = gg != null ? gg.IPAddress_F : null,
                            IPAddress_S = gg != null ? gg.IPAddress_S : null,
                            IPAddress_T = gg != null ? gg.IPAddress_T : null
                        }).ToList();
            grid1.RecordCount(list.Count);
            grid1.PageSize(pageSize);
            grid1.DataSource(list.Skip(pageSize * pageIndex).Take(pageSize), fields);
        }

        public ActionResult DoSearch(FormCollection values)
        {
            UpdateGrid(values);
            return UIHelper.Result();
        }
        public ViewResult Search()
        {
            var facs=new List<ListItem>
            {
                new ListItem("全部","")
            };
            facs.AddRange(from Manufacturer e in Enum.GetValues(typeof(Manufacturer))
                select new ListItem(e.ToString(), ((char) e).ToString()));
            ViewBag.Facs = facs.ToArray();
            return View();
        }
	}
}