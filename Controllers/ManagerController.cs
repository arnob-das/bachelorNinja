using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using messManagement.Models;
using messManagement.Models.messManagement.Models;

namespace messManagement.Controllers
{
    public class ManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Manager/CreateMess
        public ActionResult CreateMess()
        {
            if (Session["ManagerId"] == null)
            {
                return RedirectToAction("ManagerLogin", "Login");
            }
            if (Session["MessId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Manager/CreateMess
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMess(Mess mess)
        {
            if (Session["MessId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                db.Messes.Add(mess);
                db.SaveChanges();

                var messManager = new MessManager
                {
                    MessId = mess.Id,
                    ManagerId = (int)Session["ManagerId"]
                };
                db.MessManagers.Add(messManager);
                db.SaveChanges();

                Session["MessId"] = mess.Id;
                return RedirectToAction("Dashboard");
            }
            return View(mess);
        }


        public ActionResult Dashboard(int? month, int? year)
        {
            if (Session["ManagerId"] == null || Session["MessId"] == null)
            {
                return RedirectToAction("ManagerLogin", "Login");
            }

            var messId = (int)Session["MessId"];
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var totalFlatRent = db.RoomRents.Where(r => r.MessId == messId).Sum(r => (decimal?)r.Rent) ?? 0;
            var totalUtilityBills = db.UtilityBills.Where(u => u.MessId == messId && u.Month == currentMonth && u.Year == currentYear).Select(u => (decimal?)u.UtilityCost).DefaultIfEmpty(0).Sum() ?? 0;
            var totalMeals = db.MealInformations.Where(u => u.MessId == messId && u.Month == currentMonth && u.Year == currentYear).Sum(u => (decimal?)u.MealCount) ?? 0;
            var totalGroceryCost = db.GroceryCosts.Where(gc => gc.MessId == messId && gc.Month == currentMonth && gc.Year == currentYear).Select(gc => (decimal?)gc.Amount).DefaultIfEmpty(0).Sum() ?? 0;
            decimal mealRate = totalMeals > 0 ? totalGroceryCost / totalMeals : 0;

            var members = db.MessMembers.Where(mm => mm.MessId == messId).Select(mm => mm.UserId).ToList();
            var memberDetails = new List<MemberCostViewModel>();

            foreach (var userId in members)
            {
                var user = db.Users.Find(userId);
                var roomRent = db.RoomRents.FirstOrDefault(r => r.MessId == messId && r.UserId == userId)?.Rent ?? 0;
                var userMeals = db.MealInformations.Where(m => m.MessId == messId && m.UserId == userId && m.Month == currentMonth && m.Year == currentYear).Sum(m => (decimal?)m.MealCount) ?? 0;
                var userMealCost = userMeals * mealRate;
                var userUtilityCost = totalUtilityBills / members.Count; 

                var totalUserGroceryCost = db.GroceryCosts
                    .Where(gc => gc.UserId == userId && gc.MessId == messId && gc.Month == currentMonth && gc.Year == currentYear)
                    .Select(gc => (decimal?)gc.Amount)
                    .DefaultIfEmpty(0)
                    .Sum() ?? 0;

                var totalCost = roomRent + userUtilityCost + userMealCost- totalUserGroceryCost; 

                memberDetails.Add(new MemberCostViewModel
                {
                    UserName = user.FullName,
                    RoomRent = roomRent,
                    TotalMeals = userMeals,
                    MealCost = userMealCost,
                    UtilityCost = userUtilityCost,
                    GroceryCost = totalUserGroceryCost, 
                    TotalCost = totalCost
                });
            }


            ViewBag.TotalFlatRent = totalFlatRent;
            ViewBag.TotalUtilityBills = totalUtilityBills;
            ViewBag.TotalMeals = totalMeals;
            ViewBag.TotalGroceryCost = totalGroceryCost;
            ViewBag.MealRate = mealRate;
            ViewBag.SelectedMonth = currentMonth;
            ViewBag.SelectedYear = currentYear;
            ViewBag.MemberDetails = memberDetails;

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
            if (Session["ManagerId"] == null || Session["MessId"] == null)
            {
                return RedirectToAction("ManagerLogin", "Login");
            }

            var messId = (int)Session["MessId"];

            var userIdsInMess = db.MessMembers
                .Where(mm => mm.MessId == messId)
                .Select(mm => mm.UserId)
                .ToList();

            var usersInMess = db.Users
                .Where(u => userIdsInMess.Contains(u.Id))
                .ToList();

            return View(new SetRoomRentViewModel { Users = usersInMess });
        }

        public ActionResult SetUserRent(int userId)
        {
            if (Session["ManagerId"] == null || Session["MessId"] == null)
            {
                return RedirectToAction("ManagerLogin", "Login");
            }

            var messId = (int)Session["MessId"];

            var user = db.Users.Find(userId);

            var existingRoomRent = db.RoomRents.FirstOrDefault(rr => rr.MessId == messId && rr.UserId == userId);

            var model = new SetUserRentViewModel
            {
                UserId = userId,
                FullName = user.FullName,
                Rent = existingRoomRent != null ? (decimal)existingRoomRent.Rent : 0
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetUserRent(SetUserRentViewModel model)
        {
            if (Session["ManagerId"] == null || Session["MessId"] == null)
            {
                return RedirectToAction("ManagerLogin", "Login");
            }

            var messId = (int)Session["MessId"];

            var existingRoomRent = db.RoomRents.FirstOrDefault(rr => rr.MessId == messId && rr.UserId == model.UserId);

            if (existingRoomRent != null)
            {
                existingRoomRent.Rent = model.Rent; 
            }
            else
            {
                var newRoomRent = new RoomRent
                {
                    MessId = messId,
                    UserId = model.UserId,
                    Rent = model.Rent
                };
                db.RoomRents.Add(newRoomRent); 
            }

            try
            {
                db.SaveChanges();
                TempData["SuccessMessage"] = "Room rent has been successfully set.";
                return RedirectToAction("SetRoomRent"); 
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There was an error setting the room rent. Please try again.";
                return View(model);
            }
        }



    }
}
