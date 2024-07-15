using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace messManagement.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult CreateMess()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult AssignUtilityBills()
        {
            return View();
        }

        public ActionResult ApproveUsers()
        {
            return View();
        }
        public ActionResult SetRoomRent()
        {
            return View();
        }
    }
}