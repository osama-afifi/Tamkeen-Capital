using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeSheetApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Sheet");
            return RedirectToAction("LogIn", "User");
        }

    }
}
