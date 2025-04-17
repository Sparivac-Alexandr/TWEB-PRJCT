using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using taskore.Models.Auth;
using taskoreBusinessLogic;
using taskoreBusinessLogic.Interfaces;
using taskoreDomain.Enteties.User;

namespace taskore.Controllers
{
    public class AuthController : Controller
    {

        private readonly IAuth _auth;
        //Get Session
        private readonly ISesion _session;
        public AuthController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSesionBL();
            _auth = bl.GetAuthBL();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SignIn(UserLoginSession login)
        {
            if (ModelState.IsValid)
            {
                UserLoginSession data = new UserLoginSession
                {
                    Email = login.Email,
                    Password = login.Password,
                    LoginIp = Request.UserHostAddress,
                    LoginDataTime = DateTime.Now
                };

                LoginResult userLogin = (LoginResult)_session.UserLogin(data);
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

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult ForgotPass()
        {
            return View();
        }

        // GET: Auth

        [HttpPost]
        public ActionResult Auth(UserDataLogin login)
        {

            var data = new UserLoginDTO
            {
                Email = login.Email,
                Password = login.Password,
                UserIp = "localhost"
            };

            string token = _auth.UserAuthLogic(data);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(UserDataRegister register)
        {
            Debug.WriteLine(register.Email);
            
            // Check if email already exists before proceeding
            if (_session.CheckEmailExists(register.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered. Please use a different email address.");
                return View(register);
            }
            
            // Only proceed if model is valid
            if (ModelState.IsValid)
            {
                var data = new UserRegisterDTO
                {
                    Email = register.Email,
                    Password = register.Password,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    Role = register.Role
                };

                bool isRegistered = _auth.UserRegisterLogic(data);

                if (isRegistered)
                {
                    TempData["SuccessMessage"] = "Account created successfully! Please sign in.";
                    return RedirectToAction("SignIn");
                }
                else
                {
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                }
            }
            
            // If we got this far, something failed, redisplay form
            return View(register);
        }

    }
}
