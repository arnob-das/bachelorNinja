using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using messManagement.Models;

namespace messManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Login
        public ActionResult UserLogin()
        {
            if (Session["isAuthenticated"] != null && (bool)Session["isAuthenticated"])
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult ManagerLogin()
        {
            if (Session["isAuthenticated"] != null && (bool)Session["isAuthenticated"])
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == Email && u.Password == Password);
            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["UserName"] = user.FullName;
                Session["role"] = "user";

                var userMess = _context.MessMembers
                    .SingleOrDefault(um => um.UserId == user.Id);

                if (userMess != null)
                {
                    Session["MessId"] = userMess.MessId;
                }

                Session["isAuthenticated"] = true;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }
        }

        [HttpPost]
        public ActionResult ManagerLogin(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var manager = _context.Managers.SingleOrDefault(m => m.Email == Email && m.Password == Password);
            if (manager != null)
            {
                Session["ManagerId"] = manager.Id;
                Session["ManagerName"] = manager.FullName;
                Session["role"] = "manager";

                var managerMess = _context.MessManagers
                    .SingleOrDefault(mm => mm.ManagerId == manager.Id);

                if (managerMess != null)
                {
                    Session["MessId"] = managerMess.MessId; 
                }

                Session["isAuthenticated"] = true;
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
