using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace taskore.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult ExplorePage()
        {
            return View();
        }

        public ActionResult ChatPage()
        {
            return View();
        }

        public ActionResult CreateProjectPage() {
            
            return View();
        }
        public ActionResult MyProfile()
        {
            return View();
        }
        public ActionResult Terms()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult Privacy()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
    }
}
