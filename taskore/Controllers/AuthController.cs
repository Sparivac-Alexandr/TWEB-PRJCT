using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using taskoreBusinessLogic;
using taskoreBusinessLogic.Interfaces;
using taskoreDomain.Enteties.User;

namespace taskore.Controllers
{
    public class AuthController : Controller
    {

        private readonly ISesion _session;
            public AuthController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSesionBL();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SignIn(UserLoginData login)
        {
            if (ModelState.IsValid)
            {
                UserLoginData data = new UserLoginData
                {
                    Email = login.Email,
                    Password = login.Password,
                    LoginIp = Request.UserHostAddress,
                    LoginDataTime = DateTime.Now
                };

                var userLogin = _session.UserLogin(data);
                if (userLogin.Status)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", userLogin.StatusMsg);
                    return View();
                }
            }
            return View();  
        }

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
    }
}