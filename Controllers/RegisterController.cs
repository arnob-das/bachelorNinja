using messManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using messManagement.Models;

namespace messManagement.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Register
        public ActionResult UserRegister()
        {
            if (Session["isAuthenticated"] != null && (bool)Session["isAuthenticated"])
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult ManagerRegister()
        {
            if (Session["isAuthenticated"] != null && (bool)Session["isAuthenticated"])
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRegister(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FullName = model.FullName,
                    PhoneNo = model.PhoneNo,
                    NidNo = model.NidNo,
                    Email = model.Email,
                    Password = model.Password // Consider hashing the password before saving
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ManagerRegister(ManagerRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var manager = new Manager
                {
                    FullName = model.FullName,
                    PhoneNo = model.PhoneNo,
                    NidNo = model.NidNo,
                    Email = model.Email,
                    Password = model.Password // Consider hashing the password before saving
                };

                _context.Managers.Add(manager);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
