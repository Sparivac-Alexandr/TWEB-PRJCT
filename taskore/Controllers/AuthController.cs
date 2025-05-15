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
using taskoreHelpers;

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
                    // Stocăm informațiile utilizatorului în sesiune
                    Session["UserId"] = userLogin.UserId;
                    Session["UserFullName"] = $"{userLogin.FirstName} {userLogin.LastName}";
                    Session["UserEmail"] = userLogin.Email;
                    
                    // Creează și setează cookie-ul pentru autentificare persistentă
                    string encryptedEmail = taskoreHelpers.CoockieGenerator.Create(userLogin.Email);
                    HttpCookie authCookie = new HttpCookie("X-KEY", encryptedEmail);
                    authCookie.Expires = DateTime.Now.AddDays(30); // Cookie valid for 30 days
                    authCookie.HttpOnly = true; // Makes the cookie accessible only by the web server
                    Response.Cookies.Add(authCookie);
                    
                    // Redirect către pagina MyProfile
                    return RedirectToAction("MyProfile", "Main");
                }
                else
                {
                    ModelState.AddModelError("", userLogin.StatusMsg.Message);
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

        // GET: SignOut - Acțiune pentru deconectare
        public ActionResult SignOut()
        {
            // Curățăm datele din sesiune
            Session.Clear();
            
            // Șterge cookie-ul de autentificare
            if (Request.Cookies["X-KEY"] != null)
            {
                var cookie = new HttpCookie("X-KEY")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }
            
            // Redirecționăm către pagina de autentificare
            TempData["SuccessMessage"] = "You have been successfully signed out.";
            return RedirectToAction("SignIn");
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
                    // Auto login the user after registration
                    UserLoginSession loginData = new UserLoginSession
                    {
                        Email = register.Email,
                        Password = register.Password,
                        LoginIp = Request.UserHostAddress,
                        LoginDataTime = DateTime.Now
                    };

                    LoginResult userLogin = (LoginResult)_session.UserLogin(loginData);
                    if (userLogin.Status)
                    {
                        // Store user info in session
                        Session["UserId"] = userLogin.UserId;
                        Session["UserFullName"] = $"{userLogin.FirstName} {userLogin.LastName}";
                        Session["UserEmail"] = userLogin.Email;
                        
                        // Create and set authentication cookie
                        string encryptedEmail = taskoreHelpers.CoockieGenerator.Create(userLogin.Email);
                        HttpCookie authCookie = new HttpCookie("X-KEY", encryptedEmail);
                        authCookie.Expires = DateTime.Now.AddDays(30); // Cookie valid for 30 days
                        authCookie.HttpOnly = true;
                        Response.Cookies.Add(authCookie);
                        
                        return RedirectToAction("MyProfile", "Main");
                    }
                    else
                    {
                        // If auto-login fails, still consider registration successful
                        TempData["SuccessMessage"] = "Account created successfully! Please sign in.";
                        return RedirectToAction("SignIn");
                    }
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
