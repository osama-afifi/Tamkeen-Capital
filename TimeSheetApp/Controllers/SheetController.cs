using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;

namespace TimeSheetApp.Controllers
{
    public class SheetController : Controller
    {
        //
        // GET: /Sheet/
        private db8bc58a7c5b7347cbb656a21900e60022Entities db = new db8bc58a7c5b7347cbb656a21900e60022Entities();
        public ActionResult Index(string email = "ME")
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogIn", "User");
            if (email != "ME" && User.Identity.Name.Split('-')[1] == (false).ToString())
                return RedirectToAction("Index", "Home");
            if (email == "ME" && !HasWeeklySheet())
                return Create(new Sheet());

            if (email == "ME")
                ViewBag.Owner = User.Identity.Name.Split('-')[0];

            if (!HasWeeklySheet())
                return Create(new Sheet());

            //LINQ Query to get All Sheets
            IEnumerable<TimeSheetApp.Models.Sheet> query =
                            from sheet in db.Sheets
                            join user in db.SystemUsers
                            on sheet.Email equals user.Email
                            select new TimeSheetApp.Models.Sheet
                            {
                                SheetId = sheet.SheetId,
                                StartDate = sheet.DateStarted,
                                Email = sheet.Email,
                                ApprovedDays = sheet.ApprovedDays,
                                UserModel = new Models.UserModel
                                {
                                    Admin = user.Admin,
                                    Email = user.Email,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Mobile = user.Mobile,
                                    LastLogin = DateTime.Now
                                }
                            };
            if (email == "ME")
                query = query.Where(u => u.Email == User.Identity.Name.Split('-')[0]);
            else if (email != "ALL")
                query = query.Where(u => u.Email == email);

            query = query.OrderByDescending(c => c.StartDate);
            return View(query);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Sheet NewSheet)
        {
            if (ModelState.IsValid)
            {
                var sheet = db.Sheets.Create();
                sheet.Email = User.Identity.Name.Split('-')[0];
                sheet.ApprovedDays = "0000000";
                sheet.DateStarted = LastSunday();
                sheet.DateStarted = sheet.DateStarted.Subtract(new TimeSpan(sheet.DateStarted.Hour, sheet.DateStarted.Minute, 0));
                db.Sheets.Add(sheet);
                db.SaveChanges();

                return RedirectToAction("Index", "Sheet");
            }

            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }

        private int GetMaxSheetID()
        {
            IEnumerable<TimeSheetApp.Models.Sheet> maxIDs =
                         from sheet in db.Sheets
                         select new TimeSheetApp.Models.Sheet
                         {
                             SheetId = sheet.SheetId
                         };
            int maxID = 0;
            foreach (var sheet in maxIDs)
                if (sheet.SheetId > maxID)
                    maxID = sheet.SheetId;

            return maxID;
        }

        private bool HasWeeklySheet()
        {
            var email = User.Identity.Name.Split('-')[0];
            if (email == null) return true;
            DateTime Current = DateTime.Now;
            var lastDate =
                        from sheet in db.Sheets
                        where sheet.Email == email
                        select sheet;
            DateTime maxDate = DateTime.MinValue;
            foreach (var s in lastDate)
                if (s.DateStarted > maxDate)
                    maxDate = s.DateStarted;
            if (lastDate == null || maxDate == DateTime.MinValue)
                return false;
            return InSameWeek(maxDate);
        }

        private bool InSameWeek(DateTime SheetDate)
        {
            if (SheetDate.AddDays(+7) > DateTime.Now)
                return true;
            return false;
        }
        private DateTime LastSunday()
        {
            DateTime lastSunday = DateTime.Now;
            while (lastSunday.DayOfWeek != DayOfWeek.Sunday)
            {
                lastSunday = lastSunday.AddDays(-1);
            }
            return lastSunday;
        }
        //public ActionResult Delete(int ID)
        //{
        //    deleteSheet(ID);
        //    db.SaveChanges();
        //    return View();
        //}
        public void deleteSheet(int ID)
        {
            var sheet = db.Sheets.Find(ID);
            if (sheet == null) return;
           
            var query = from T in db.Tasks
                        where T.SheetId == sheet.SheetId
                        select T;
            foreach (var T in query.ToList())
                db.Entry(T).State = System.Data.EntityState.Deleted;
        
            db.Entry(sheet).State = System.Data.EntityState.Deleted;
            db.SaveChanges();
        }
    }
}
