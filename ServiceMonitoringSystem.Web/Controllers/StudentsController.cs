using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FineUIMvc;
using Newtonsoft.Json.Linq;
using ServiceMonitoringSystem.Web.Models;

namespace ServiceMonitoringSystem.Web.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private StudentDbContext db = new StudentDbContext();

        // GET: Students
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="Grid1_fields"></param>
        private void UpdateGrid(JArray Grid1_fields)
        {
            UIHelper.Grid("Grid1").DataSource(db.Students.ToList(), Grid1_fields);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Window1_Close(JArray Grid1_fields)
        {
            UpdateGrid(Grid1_fields);

            return UIHelper.Result();
        }


        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnCreate_Click([Bind(Include = "ID,Name,Gender,Major,EntranceDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }



        // GET: Students/Edit/?studentId=5
        public ActionResult Edit(int studentId)
        {
            Student student = db.Students.Find(studentId);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnEdit_Click([Bind(Include = "ID,Name,Gender,Major,EntranceDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();

                // 关闭本窗体（触发窗体的关闭事件）
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            }

            return UIHelper.Result();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grid1_Delete(JArray selectedRows, JArray Grid1_fields)
        {
            foreach (JToken rowId in selectedRows)
            {
                Student student = db.Students.Find(Convert.ToInt32(rowId));
                db.Students.Remove(student);
            }
            db.SaveChanges();

            UpdateGrid(Grid1_fields);

            return UIHelper.Result();
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}