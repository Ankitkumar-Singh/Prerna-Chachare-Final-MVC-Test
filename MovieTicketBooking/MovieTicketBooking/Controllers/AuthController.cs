using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Controllers
{
    public class AuthController : Controller
    {
        #region Database Context
        /// <summary>The database</summary>
        private MovieTicketContext db = new MovieTicketContext();
        #endregion

        #region Home Page
        /// <summary>Homes this instance.</summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Home()
        {
            return View();
        }
        #endregion

        #region Login User
        /// <summary>Logins this instance.</summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>Logins the specified user.</summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User user)
        {
            using (var context = new MovieTicketContext())
            {
                if (context.Users.Any(x => x.Email.Equals(user.Email, StringComparison.Ordinal) && x.Password.Equals(user.Password, StringComparison.Ordinal)))
                {
                    User userDetail = context.Users.Single(x => x.Email == user.Email);
                    Session["UserId"] = userDetail.UserId;
                    Session["UserEmail"] = userDetail.Email;
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                    return RedirectToDefault();
                }
            }
            ModelState.AddModelError("", "Invalid email and password");
            return View();
        }
        #endregion

        #region Register User
        /// <summary>Registers this instance.</summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>Registers the specified user.</summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login", "Auth");
            }
            return View(user);
        }
        #endregion

        #region Check User Role and redirect user to respective page
        /// <summary>Redirects to default.</summary>
        /// <returns></returns>
        public ActionResult RedirectToDefault()
        {
            String[] roles = Roles.GetRolesForUser();
            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Home", "Auth");
            }
            else if (roles.Contains("User"))
            {
                return RedirectToAction("Home", "Auth");
            }
            else
            {
                return RedirectToAction("Home", "Auth");
            }
        }
        #endregion

        #region Log off User
        /// <summary>Logoffs this instance.</summary>
        /// <returns></returns>
        [Authorize(Roles = "User,Admin")]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Home", "Auth");
        }
        #endregion
    }
}