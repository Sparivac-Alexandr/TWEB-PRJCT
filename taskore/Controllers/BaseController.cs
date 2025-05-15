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
    public class BaseController : Controller
    {
        private readonly ISesion _session;

        public BaseController()
        {
            var bl = new BusinessLogic();
            _session = bl.GetSesionBL();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            // Check login status on every action
            SessionStatus();
        }

        public void SessionStatus()
        {
            var apiCookie = Request.Cookies["X-KEY"];
            if (apiCookie != null)
            {
                var profile = _session.GetUserByCookie(apiCookie.Value);
                if (profile != null)
                {
                    // User found, update session data
                    Session["LoginStatus"] = "login";
                    Session["UserId"] = profile.UserId;
                    Session["UserFullName"] = $"{profile.FirstName} {profile.LastName}";
                    Session["UserEmail"] = profile.Email;
                }
                else
                {
                    // Invalid cookie, clear session and remove cookie
                    Session.Clear();
                    if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("X-KEY"))
                    {
                        var cookie = ControllerContext.HttpContext.Request.Cookies["X-KEY"];
                        if (cookie != null)
                        {
                            cookie.Expires = DateTime.Now.AddDays(-1);
                            ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                        }
                    }

                    Session["LoginStatus"] = "logout";
                }
            }
            else
            {
                // No cookie, check if session exists
                if (Session["UserId"] == null)
                {
                    Session["LoginStatus"] = "logout";
                }
            }
        }
    }
}