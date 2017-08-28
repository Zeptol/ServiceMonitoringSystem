using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FineUIMvc;
using Newtonsoft.Json.Linq;
using ServiceMonitoringSystem.Web.Models;

namespace ServiceMonitoringSystem.Web.Controllers
{
    [Authorize]
    public class StudentsSearchPagingController : BaseController
    {
        private StudentDbContext db = new StudentDbContext();

        private ListItem[] GetMajorList()
        {
            var majors = db.Students.OrderBy(m => m.Major).Select(m => m.Major).Distinct();

            var items = new List<ListItem>();
            items.Add(new ListItem
            {
                Text = "全部专业",
                Value = "ALL"
            });
            foreach (string major in majors)
            {
                items.Add(new ListItem
                {
                    Text = major,
                    Value = major
                });
            }
            return items.ToArray();
        }

        private static readonly int PAGE_SIZE = 3;

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }

        // 获取分页数据
        private List<Student> GetPagedDataSource(IQueryable<Student> students, int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }

            return students.OrderBy(m => m.Name).Skip(pageIndex * PAGE_SIZE).Take(PAGE_SIZE).ToList();
        }

        // 更新表格数据
        private void UpdateGrid(FormCollection values)
        {
            string name = values["tbxSearchName"];
            string major = values["ddlSearchMajor"];
            JArray fields = JArray.Parse(values["Grid1_fields"]);
            int pageIndex = Convert.ToInt32(values["Grid1_pageIndex"]);

            var students = db.Students as IQueryable<Student>;

            if (!String.IsNullOrEmpty(name))
            {
                students = students.Where(m => m.Name.Contains(name));
            }

            if (major != "ALL")
            {
                students = students.Where(m => m.Major == major);
            }

            var recordCount = students.Count();

            var grid1 = UIHelper.Grid("Grid1");
            grid1.RecordCount(recordCount);
            grid1.DataSource(GetPagedDataSource(students, pageIndex, recordCount), fields);
        }



        // GET: Students
        public ActionResult Index()
        {
            ViewBag.MajorList = GetMajorList();

            var students = db.Students as IQueryable<Student>;
            var recordCount = students.Count();

            ViewBag.Grid1RecordCount = recordCount;
            ViewBag.Grid1PageSize = PAGE_SIZE;

            return View(GetPagedDataSource(students, 0, recordCount));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Window1_Close(FormCollection values)
        {
            UpdateGrid(values);

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
        public ActionResult Grid1_Delete(JArray selectedRows, FormCollection values)
        {
            foreach (string rowId in selectedRows)
            {
                Student student = db.Students.Find(Convert.ToInt32(rowId));
                db.Students.Remove(student);
            }
            db.SaveChanges();

            UpdateGrid(values);

            return UIHelper.Result();
        }


        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoSearchPaging(FormCollection values)
        {
            UpdateGrid(values);

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