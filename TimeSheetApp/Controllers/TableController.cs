using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace TimeSheetApp.Controllers
{
    public class TableController : Controller
    {

        //
        // GET: /Table/
        private db8bc58a7c5b7347cbb656a21900e60022Entities db = new db8bc58a7c5b7347cbb656a21900e60022Entities();
        private int currentSheet = -1;
        public ActionResult Index(int id = 0)
        {

            var email = User.Identity.Name.Split('-')[0];
            if (email == null || id == 0)
                return RedirectToAction("Index", "Home");
            ViewBag.SheetID = id;
            //        currentSheet = id;
            Session["SheetID"] = id;
            currentSheet = id;
            Models.Sheet sheet = SingleSheet(id);

            if (sheet.Email != User.Identity.Name.Split('-')[0] && User.Identity.Name.Split('-')[1] == (false).ToString())
                return RedirectToAction("Index", "Home");

            if (sheet == null)
                return RedirectToAction("Index", "Home");
            string holiday = WeekHolidays(sheet.StartDate);

            TimeSheetApp.Models.TableViewModel table = new TimeSheetApp.Models.TableViewModel
            {
                Sheet = sheet,
                Holiday = holiday,
                Approved = sheet.ApprovedDays
            };
            return View(table);
        }

        [HttpGet]
        public ActionResult DayChange(int sheetID, int day = -1)
        {
            if (day == -1)
                return RedirectToAction("Index", "Home");
            ViewData.Add("SheetId", sheetID);
            HttpCookie UserInfo = Request.Cookies["UserInfo"];
            UserInfo["TargetSheet"] = sheetID.ToString();
            Response.Cookies.Add(UserInfo);
            var Days =
                        from T in db.Tasks
                        where T.SheetId == sheetID
                        select new TimeSheetApp.Models.DayViewModel
                        {
                            dayTask = new TimeSheetApp.Models.Task
           {

               Description = T.TaskDescription,
               HoursDone = T.HoursWorked,
               Notes = T.TaskNotes,
               SheetId = T.SheetId,
               TaskId = T.TaskId
           }
            ,
                            hourValue = 0
                        };

            List<TimeSheetApp.Models.DayViewModel> list = new List<TimeSheetApp.Models.DayViewModel>();
            foreach (TimeSheetApp.Models.DayViewModel D in Days)
                list.Add(new TimeSheetApp.Models.DayViewModel { dayTask = D.dayTask, hourValue = (int)(D.dayTask.HoursDone[0] - '0') });

            TimeSheetApp.Models.DayViewModelList ret = new TimeSheetApp.Models.DayViewModelList();
            ret.DayTasks = list;
            ret.day = day;
            return View(ret);
        }

        [HttpPost]
        public ActionResult DayChange(TimeSheetApp.Models.DayViewModelList Days)
        {

            int id = int.Parse(Request.Cookies["UserInfo"]["TargetSheet"].ToString());
            int day = Days.day;
            int sheetId = id;
            List<char> tasks = new List<char>();
            if (ModelState.IsValid)
            {
                foreach (Models.DayViewModel D in Days.DayTasks)
                {
                    tasks.Add((char)(D.hourValue + '0'));

                }


                var taskList = from T in db.Tasks
                               where T.SheetId == sheetId
                               select T;
                int count = 0;
                foreach (var T in taskList)
                {
                    T.HoursWorked = ReplaceAtIndex(day, tasks[count], T.HoursWorked);
                    db.Entry(T).State = System.Data.EntityState.Modified;
                    count++;
                }
                db.SaveChanges();

                return RedirectToAction("Index", "Table", new { id = sheetId });
            }
            return View();
        }

        [HttpGet]
        public ActionResult Approve(int day = -1, int approve = -1)
        {

            int sheetId = int.Parse(Session["SheetID"].ToString());
            if (day == -1 || approve == -1)
                return RedirectToAction("Index", "Table", new { id = sheetId });
            char choice = '1';
            if (approve == 0)
                choice = '2';

            var EditedSheet = db.Sheets.Find(sheetId);
            string App = EditedSheet.ApprovedDays;
            EditedSheet.ApprovedDays = ReplaceAtIndex(day, choice, App);
            db.Entry(EditedSheet).State = System.Data.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Table", new { id = sheetId });
        }

        private Models.Sheet SingleSheet(int id)
        {
            IEnumerable<TimeSheetApp.Models.Sheet> sheets = from sheet in db.Sheets
                                                            join user in db.SystemUsers
                                                            on sheet.Email equals user.Email
                                                            where sheet.SheetId == id
                                                            select new Models.Sheet
                                                            {
                                                                SheetId = sheet.SheetId,
                                                                StartDate = sheet.DateStarted,
                                                                Email = sheet.Email,
                                                                ApprovedDays = sheet.ApprovedDays,

                                                                UserModel = new Models.UserModel
                                                                {
                                                                    FirstName = user.FirstName,
                                                                    LastLogin = user.LastLogin,
                                                                    LastName = user.LastName,
                                                                    Mobile = user.Mobile,
                                                                    Email = user.Email,
                                                                    Admin = user.Admin
                                                                },
                                                                Task =
                                                             (from task in db.Tasks
                                                              //join task in db.Tasks
                                                              //on sheet.SheetId equals task.SheetId
                                                              where task.SheetId == id
                                                              select new Models.Task
                                                              {
                                                                  TaskId = task.TaskId,
                                                                  Description = task.TaskDescription,
                                                                  Notes = task.TaskNotes,
                                                                  SheetId = task.SheetId,
                                                                  HoursDone = task.HoursWorked
                                                              }
                                                                )
                                                            };
            Models.Sheet singleSheet = null;
            foreach (Models.Sheet s in sheets)
            {
                singleSheet = s;
                break;
            }

            return singleSheet;

        }

        private string WeekHolidays(DateTime start)
        {
            string holiday = "0000011";
            DateTime current = start;

            for (int i = 0; i < 7; i++)
            {
                var email = User.Identity.Name.Split('-')[0];
                var query = from scheduledholiday in db.ScheduledHolidays
                            //where scheduledholiday.UserEmail==email && scheduledholiday.HolidayDay.Day == current.Day && scheduledholiday.HolidayDay.Month == current.Month
                            select scheduledholiday;
                bool isHoliday = false;
                foreach (var SH in query)
                {
                    if (InRange(SH.HolidayStart, SH.HolidayEnd, current))
                    {
                        isHoliday = true;
                        break;
                    }
                }

                if (isHoliday)
                    holiday = ReplaceAtIndex(i, '1', holiday);

                if (holiday[i] == '0')
                {

                    var casual = from casualholiday in db.CasualHolidays
                                 where casualholiday.UserEmail == email && casualholiday.HolidayDay.Day == current.Day && casualholiday.HolidayDay.Month == current.Month && casualholiday.HolidayDay.Year == current.Year
                                 select casualholiday;
                    if (casual != null && casual.Count() != 0)
                        holiday = ReplaceAtIndex(i, '1', holiday);
                }
                current = current.AddDays(1);
            }
            return holiday;
        }
        private string ReplaceAtIndex(int i, char value, string word)
        {
            char[] letters = word.ToCharArray();
            letters[i] = value;
            return string.Join("", letters);
        }

        private bool InRange(DateTime start, DateTime end, DateTime day)
        {
            start = start.AddHours(-start.Hour);
            start = start.AddMinutes(-start.Minute + 1);

            end = end.AddHours(-end.Hour);
            end = end.AddMinutes(-end.Minute + 1);
            end = end.AddHours(23);

            day = day.AddHours(-day.Hour);
            day = day.AddMinutes(-day.Minute + 1);
            day = day.AddHours(12);

            if (day <= end && day >= start)
                return true;
            return false;
        }
    }
}
