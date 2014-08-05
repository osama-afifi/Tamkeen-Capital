using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TimeSheetApp.Controllers
{
    public class CasualHolidayController : Controller
    {
        //
        // GET: /CasualHoliday/
        private db8bc58a7c5b7347cbb656a21900e60022Entities db = new db8bc58a7c5b7347cbb656a21900e60022Entities();

        public ActionResult Index(string email = "ME")
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogIn", "User");
            if (email != "ME" && User.Identity.Name.Split('-')[1] == (false).ToString())
                return RedirectToAction("Index", "Home");
            IEnumerable<TimeSheetApp.Models.CasHolidays> query =
               from casholiday in db.CasualHolidays
               join user in db.SystemUsers
                on casholiday.UserEmail equals user.Email
               select new TimeSheetApp.Models.CasHolidays
               {
                   CasualHolidayId = casholiday.CasualHolidayId,
                   Email = casholiday.UserEmail,
                   Reason = casholiday.Reason,
                   HolidayDate = casholiday.HolidayDay,
                   UserModel = new TimeSheetApp.Models.UserModel
                   {
                       FirstName = user.FirstName,
                       Email = user.Email,
                       Admin = user.Admin,
                       LastName = user.LastName,
                       Mobile = user.Mobile,
                       LastLogin = user.LastLogin
                   }

               };
            if (email == "ME")
                query = query.Where(u => u.Email == User.Identity.Name.Split('-')[0]);
            else if (email != "ALL")
                query = query.Where(u => u.Email == email);
            ViewBag.UserEmail = email;
            return View(query.OrderByDescending(c => c.HolidayDate));
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
        public ActionResult Create(TimeSheetApp.Models.CasHolidays CasualHoliday)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("LogIn", "User");
            if (ModelState.IsValidField("HolidayDate") && ModelState.IsValidField("Reason"))
            {
                var casual = db.CasualHolidays.Create();
                casual.HolidayDay = CasualHoliday.HolidayDate;
                casual.Reason = CasualHoliday.Reason;
                casual.UserEmail = Session["TargetEmail"].ToString();
                db.CasualHolidays.Add(casual);
                db.SaveChanges();
                var user = db.SystemUsers.Find(User.Identity.Name.Split('-')[0]);

                // Sending Notification Email
                Helping.SendEmail(User.Identity.Name.Split('-')[0], user.FirstName + " " + user.LastName, "Casual Vacation Succesfully Created",
                    "Dear " + user.FirstName + "\n\n" + "You Succesfully created a New Casual Holiday with the Date : " + casual.HolidayDay.Date.ToString("dd MMM yyyy") + ".\n"
                    + "And your Justification was: " + casual.Reason + ".\n\n"
                    + "Best Regards\n"
                    + " Tamkeen Capital Time Management System\n"
                    );

                var query = from admins in db.SystemUsers
                            where admins.Admin == true
                            select admins;
          //      foreach (var A in query)

                {
                    Helping.SendEmail("timesheets@tamkeencapital.com", "Administrator" , user.FirstName + " has created a Casual Vacation",
        "Dear Administrator"+ "\n\n" + user.FirstName + " " + user.LastName + " has created a new Casual Holiday with the Date : " + casual.HolidayDay.Date.ToString("dd MMM yyyy") + ".\n"
        + "And his Justification was: " + casual.Reason + ".\n\n"
        + "Best Regards\n"
        + " Tamkeen Capital Time Management System\n"
        );
                }

                return RedirectToAction("Index", "CasualHoliday");
            }

            return View();
        }

        public ActionResult Delete(int ID = 0)
        {
            deleteCasual(ID);
            return RedirectToAction("Index", "Home");
        }
        private void deleteCasual(int ID)
        {
            var cas = db.CasualHolidays.Find(ID);
            if (cas == null) return;
            db.Entry(cas).State = System.Data.EntityState.Deleted;
            db.SaveChanges();
        }


    }
}
