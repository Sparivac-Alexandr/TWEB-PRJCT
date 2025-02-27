using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace taskore.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult ForgotPass()
        {
            return View();
        }

        public ActionResult MyProfile()
        {
            return View();
        }
    }
}