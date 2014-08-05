using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeSheetApp.Controllers
{
    public class ScheduledHolidayController : Controller
    {
        //
        // GET: /ScheduledHoliday/
        private db8bc58a7c5b7347cbb656a21900e60022Entities db = new db8bc58a7c5b7347cbb656a21900e60022Entities();

        public ActionResult Index(string email = "ME")
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogIn", "User");
            if (email != "ME" && User.Identity.Name.Split('-')[1] == (false).ToString())
                return RedirectToAction("Index", "Home");
            ViewBag.UserEmail = email;
              IEnumerable<TimeSheetApp.Models.SchHolidays> query =
                            from scholiday in db.ScheduledHolidays
                            join user in db.SystemUsers
                            on scholiday.UserEmail equals user.Email
                            select new TimeSheetApp.Models.SchHolidays
                            {
                                ScheduledHolidayId = scholiday.ScheduledHolidayId ,
                                 Reason = scholiday.Reason ,
                                 HolidayStart = scholiday.HolidayStart ,
                                 HolidayEnd = scholiday.HolidayEnd ,
                                 UserEmail = scholiday.UserEmail ,

                                UserModel = new Models.UserModel 
                                {
                                FirstName = user.FirstName ,
                                LastName = user.LastName ,
                                LastLogin = user.LastLogin ,
                                Mobile = user.Mobile ,
                                Admin = user.Admin                                
                                }
                            };
              if (email == "ME")
                  query = query.Where(u => u.UserEmail == User.Identity.Name.Split('-')[0]);
              else if (email != "ALL")
                  query = query.Where(u => u.UserEmail == email);
              return View(query.OrderByDescending(c => c.HolidayStart));
        }

        [HttpGet]
        public ActionResult Create(string email = "ME")
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogIn", "User");
            if (email != "ME" && User.Identity.Name.Split('-')[1] == (false).ToString())
                return RedirectToAction("Index", "Home");
            if (email == "ME")
                email = User.Identity.Name.Split('-')[0];
            Session["TargetEmail"] = email;
            ViewBag.TargetEmail = email;

            return View();
        }
        [HttpPost]
        public ActionResult Create(TimeSheetApp.Models.SchHolidays ScheduledHoliday)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogIn", "User");
            if (ModelState.IsValidField("HolidayStart") && ModelState.IsValidField("HolidayEnd") && ModelState.IsValidField("Reason"))
            {
                var scheduled = db.ScheduledHolidays.Create();
                scheduled.HolidayStart = ScheduledHoliday.HolidayStart.AddMinutes(1);
                scheduled.HolidayEnd = ScheduledHoliday.HolidayEnd;
                scheduled.Reason = ScheduledHoliday.Reason;
                scheduled.UserEmail = Session["TargetEmail"].ToString();
                db.ScheduledHolidays.Add(scheduled);
                db.SaveChanges();

                // Sending Notification Email
                var user = db.SystemUsers.Find(Session["TargetEmail"].ToString());
                Helping.SendEmail(user.Email, user.FirstName + " " + user.LastName, "Vacation Succesfully Scheduled",
                    "Dear " + user.FirstName + "\n\n" + "You have a New Scheduled Vacation from the Date : " + scheduled.HolidayStart.Date.ToString("dd MMM yyyy") + "to the Date : "+  scheduled.HolidayEnd.Date.ToString("dd MMM yyyy") + ".\n"
                    + "And the Reason was: " + scheduled.Reason + ".\n\n"
                    + "Best Regards\n"
                    + " Tamkeen Capital Time Management System\n"
                    );

                return RedirectToAction("Index", "ScheduledHoliday");
            }

            return View();
        }

        public ActionResult Delete(int ID)
        {
            deleteScheduled(ID);
            return RedirectToAction("Index", "Home");
        }
        private void deleteScheduled(int ID)
        {
            var sch = db.ScheduledHolidays.Find(ID);
            if (sch == null) return;
            db.Entry(sch).State = System.Data.EntityState.Deleted;
            db.SaveChanges();
        }
    }
}
