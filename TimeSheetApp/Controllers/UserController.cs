using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.Data;

namespace TimeSheetApp.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        private db8bc58a7c5b7347cbb656a21900e60022Entities db = new db8bc58a7c5b7347cbb656a21900e60022Entities();

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated || User.Identity.Name.Split('-')[1] == (false).ToString())
                return View();

            IEnumerable<TimeSheetApp.Models.UserModel> query =
                            from user in db.SystemUsers
                            select new TimeSheetApp.Models.UserModel
                            {
                                Admin = user.Admin,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastLogin = user.LastLogin,
                                LastName = user.LastName,
                                Mobile = user.Mobile
                            };
            return View(query.OrderBy(u => u.FirstName));
        }

        public ActionResult ProfileID(string email = "NULL")
        {
            if (email == null || !Request.IsAuthenticated || User.Identity.Name.Split('-')[1] == (false).ToString())
                return RedirectToAction("Index", "User");
            var user = db.SystemUsers.Find(email);
            if (user == null)
                return RedirectToAction("Index", "User");
            Models.UserModel ret = new Models.UserModel
            {
                Email = user.Email,
                Admin = user.Admin,
                FirstName = user.FirstName,
                LastLogin = user.LastLogin,
                LastName = user.LastName,
                Mobile = user.Mobile
            };

            return View(ret);
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(Models.UserModel user)
        {
            if (ModelState.IsValidField("Email") && ModelState.IsValidField("Password"))
            {
                if (IsValid(user.Email, user.Password))
                {
                   bool admin = db.SystemUsers.Find(user.Email).Admin;
                    FormsAuthentication.SetAuthCookie(user.Email+"-"+admin.ToString(), false);
                    //Session["Email"] = user.Email;
                    //    ViewBag.isAdmin = IsAdmin(user.Email);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login Email and Password Mismatch");

                }
            }
            return View(user);
        }


        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(Models.UserModel user)
        {
            if (User.Identity.Name.Split('-')[1] == (false).ToString())
                return RedirectToAction("Index", "Home");
            user.LastLogin = DateTime.Now;
            if (ModelState.IsValid)
            {
                using (var db = new db8bc58a7c5b7347cbb656a21900e60022Entities())
                {
                    if (db.SystemUsers.Find(user.Email) != null)
                        return View(user);
                    var crypto = new SimpleCrypto.PBKDF2();
                    var encryptedPass = crypto.Compute(user.Password);
                    var systemUser = db.SystemUsers.Create();

                    systemUser.Email = user.Email;

                    systemUser.Password = encryptedPass;
                    systemUser.PasswordSalt = crypto.Salt;
                    systemUser.UserId = Guid.NewGuid();
                    systemUser.FirstName = user.FirstName;
                    systemUser.LastName = user.LastName;
                    systemUser.Admin = user.Admin;
                    systemUser.Mobile = user.Mobile;
                    systemUser.LastLogin = DateTime.Now;

                    db.SystemUsers.Add(systemUser);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");

                }


            }
            return View(user);
        }
        [HttpGet]
        public ActionResult Edit()
        {
            var user = db.SystemUsers.Find(User.Identity.Name.Split('-')[0]);
            Models.UserModel ret = new Models.UserModel
            {
            FirstName = user.FirstName ,
            LastName = user.LastName ,
            Mobile = user.Mobile , 
            Password = "" , 
            Admin = user.Admin ,
            Email = user.Email ,
                     
            };
            return View(ret);
        }

        [HttpPost]
        public ActionResult Edit(Models.UserModel user)
        {

            user.LastLogin = DateTime.Now;
            if (ModelState.IsValidField("Password") && ModelState.IsValidField("FirstName") && ModelState.IsValidField("LastName") && ModelState.IsValidField("Mobile"))
            {
                using (var db = new db8bc58a7c5b7347cbb656a21900e60022Entities())
                {
                    var crypto = new SimpleCrypto.PBKDF2();
                    var encryptedPass = crypto.Compute(user.Password);
                    var systemUser = db.SystemUsers.Find(User.Identity.Name.Split('-')[0]);

                    if(systemUser==null)
                        return RedirectToAction("Index", "Home");

                    systemUser.Email = User.Identity.Name.Split('-')[0];
                    systemUser.Password = encryptedPass;
                    systemUser.PasswordSalt = crypto.Salt;
                    systemUser.FirstName = user.FirstName;
                    systemUser.LastName = user.LastName;
                    systemUser.Mobile = user.Mobile;
                    systemUser.LastLogin = DateTime.Now;

                    db.Entry(systemUser).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");

                }


            }
            return View(user);
        }

        public ActionResult PromoteAdmin(string email = "NULL")
        {
            if(email == "NULL")
                return RedirectToAction("Index", "Home");
            var user = db.SystemUsers.Find(email);
            if(user==null)
                return RedirectToAction("Index", "Home");

            user.Admin = !user.Admin;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ProfileID", "User", new { email = email });
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            //Response.Cookies.Clear();

            return RedirectToAction("Index", "Home");
        }

        private bool IsValid(string email, string password)
        {
            bool isValid = false;
            var crypto = new SimpleCrypto.PBKDF2();
            using (var db = new db8bc58a7c5b7347cbb656a21900e60022Entities())
            {
                var user = db.SystemUsers.Find(email);

                if (user != null)
                {
                    string cr = crypto.Compute(password, user.PasswordSalt);
                    if (user.Password == cr)
                    {
                        isValid = true;
                        user.LastLogin = DateTime.Now;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        
                        HttpCookie UserInfo = new HttpCookie("UserInfo");
                        
                    //    UserInfo["Email"] = user.Email;
                    //    UserInfo["Admin"] = user.Admin.ToString();
                        UserInfo["Name"] = user.FirstName + " " + user.LastName;
                        UserInfo["TargetSheet"] = "";
                        Response.Cookies.Add(UserInfo);
                    }

                }
            }

            return isValid;
        }



        private bool IsAdmin(string email)
        {
            bool isAdmin = false;
            var crypto = new SimpleCrypto.PBKDF2();
            using (var db = new db8bc58a7c5b7347cbb656a21900e60022Entities())
            {
                var user = db.SystemUsers.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    if (user.Admin == true)
                    {
                        isAdmin = true;
                    }

                }
            }
            return isAdmin;
        }
        public ActionResult Delete(string email)
        {
            deleteUser(email);
            return RedirectToAction("Index", "Home");
        }
        public void deleteUser(string email)
        {
            var user = db.SystemUsers.Find(email);
            if (user == null) return;

            var queryS = from S in db.Sheets
                         where S.Email == user.Email
                         select S;
            foreach (var S in queryS.ToList())
            {
                var queryT = from T in db.Tasks
                             where T.SheetId == S.SheetId
                             select T;

                foreach (var T in queryT.ToList())
                    db.Entry(T).State = System.Data.EntityState.Deleted;
              
                db.Entry(S).State = System.Data.EntityState.Deleted;

            }

            var querySH = from SH in db.ScheduledHolidays
                          where SH.UserEmail == user.Email
                          select SH;
            foreach (var SH in querySH.ToList())
                db.Entry(SH).State = System.Data.EntityState.Deleted;
            

            var queryCH = from CH in db.CasualHolidays
                          where CH.UserEmail == user.Email
                          select CH;
            foreach (var CH in queryCH.ToList())
                db.Entry(CH).State = System.Data.EntityState.Deleted;

            db.Entry(user).State = System.Data.EntityState.Deleted;

            db.SaveChanges();
        }
    }
}
