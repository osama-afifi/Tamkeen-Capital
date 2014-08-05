using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace TimeSheetApp.Controllers
{
    public class TaskController : Controller
    {
        //
        // GET: /Task/
        private db8bc58a7c5b7347cbb656a21900e60022Entities db = new db8bc58a7c5b7347cbb656a21900e60022Entities();

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated || Request.Cookies["UserInfo"] == null) return View();
            var email = User.Identity.Name.Split('-')[0];

            //LINQ Query to get All Tasks
            IEnumerable<TimeSheetApp.Models.Task> query =
                            from task in db.Tasks
                            join sheet in db.Sheets
                            on task.SheetId equals sheet.SheetId
                            where sheet.Email == email
                            select new TimeSheetApp.Models.Task
                            {
                                TaskId = task.TaskId , 
                                Description = task.TaskDescription ,
                                Notes = task.TaskNotes , 
                                HoursDone = task.HoursWorked ,
                                SheetId = task.SheetId,
                                Sheet = new Models.Sheet
                                {
                                    SheetId = sheet.SheetId , 
                                    Email = sheet.Email ,
                                    ApprovedDays = sheet.ApprovedDays ,
                                    StartDate = sheet.DateStarted 
                                }
                            };

            return View(query);
        }
        public ActionResult Delete(int ID = 0)
        {
            if (ID == 0) return Redirect(Request.UrlReferrer.ToString());
            var T =db.Tasks.Find(ID);
            if (T == null) return Redirect(Request.UrlReferrer.ToString());
            int sheetID = T.SheetId;
            deleteTask(ID);
            db.SaveChanges();
            return RedirectToAction("Index", "Table", new { id = sheetID });
        }
        public void deleteTask(int ID)
        {
            var task = db.Tasks.Find(ID);
            if (task == null) return;
            db.Entry(task).State = System.Data.EntityState.Deleted;
            db.SaveChanges();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(TimeSheetApp.Models.Task Task)
        {
            if (ModelState.IsValid)
            {
                var task = db.Tasks.Create();
                //task.SheetId = ID;
                task.SheetId = int.Parse(Request.Cookies["UserInfo"]["TargetSheet"].ToString());
                task.HoursWorked = "0000000";
                task.TaskDescription = Task.Description;
                task.TaskNotes = Task.Notes;
                db.Tasks.Add(task);
                db.SaveChanges();

                return RedirectToAction("Index", "Table", new { id=task.SheetId });
            }

            return View();
        }
        public ActionResult Details(int id =0)
        {
            var T = db.Tasks.Find(id);
            if(T == null)
            return HttpNotFound();
            TimeSheetApp.Models.Task ret = new TimeSheetApp.Models.Task
            {
                TaskId = T.TaskId,
                Description = T.TaskDescription,
                Notes = T.TaskDescription,
                SheetId = T.SheetId,
                HoursDone = T.HoursWorked
            };
            return View(ret);
        }

    }
}
