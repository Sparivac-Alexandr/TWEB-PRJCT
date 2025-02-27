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
            
    }
}