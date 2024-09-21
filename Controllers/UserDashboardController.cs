using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using messManagement.Models;

namespace messManagement.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserDashboardController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: UserDashboard
        public ActionResult Index(int? month, int? year)
        {
            if (Session["MessId"] == null)
            {
                return RedirectToAction("JoinMess");
            }

            int userId = (int)Session["UserId"]; 
            int messId = (int)Session["MessId"]; 

            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var roomRent = _context.RoomRents
                .Where(rr => rr.UserId == userId && rr.MessId == messId)
                .Select(rr => (decimal?)rr.Rent) 
                .FirstOrDefault() ?? 0;

            var totalUtilityCost = _context.UtilityBills
                .Where(ub => ub.MessId == messId && ub.Month == currentMonth && ub.Year == currentYear)
                .Select(ub => (decimal?)ub.UtilityCost) 
                .DefaultIfEmpty(0)
                .Sum() ?? 0; 

            var totalMembers = _context.MessMembers
                .Count(mm => mm.MessId == messId);

            decimal utilityCostPerPerson = totalMembers > 0 ? totalUtilityCost / totalMembers : 0;

            var userTotalMeals = _context.MealInformations
                .Where(mi => mi.UserId == userId && mi.MessId == messId && mi.Month == currentMonth && mi.Year == currentYear)
                .Select(mi => (int?)mi.MealCount) 
                .DefaultIfEmpty(0) 
                .Sum() ?? 0;

            var totalUserGroceryCost = _context.GroceryCosts
                .Where(gc => gc.UserId == userId && gc.MessId == messId && gc.Month == currentMonth && gc.Year == currentYear)
                .Select(gc => (decimal?)gc.Amount)
                .DefaultIfEmpty(0)
                .Sum() ?? 0;

            var totalMessGroceryCost = _context.GroceryCosts
                .Where(gc => gc.MessId == messId && gc.Month == currentMonth && gc.Year == currentYear)
                .Select(gc => (decimal?)gc.Amount)
                .DefaultIfEmpty(0)
                .Sum() ?? 0;

            var totalMessMeals = _context.MealInformations
                .Where(mi => mi.MessId == messId && mi.Month == currentMonth && mi.Year == currentYear)
                .Select(mi => (int?)mi.MealCount)
                .DefaultIfEmpty(0)
                .Sum() ?? 0;

            decimal mealRate = totalMessMeals > 0 ? totalMessGroceryCost / totalMessMeals : 0;

            decimal userMealCost = userTotalMeals * mealRate;

            decimal totalCost = (roomRent > 0 ? roomRent : 0) + utilityCostPerPerson + userMealCost - totalUserGroceryCost;

            var model = new UserDashboardViewModel
            {
                RoomRent = roomRent > 0 ? roomRent : 0,
                UtilityCost = utilityCostPerPerson,
                MealRate = mealRate,
                TotalCost = totalCost,
                TotalMeals = userTotalMeals,
                UserMealCost = userMealCost,
                TotalUserGroceryCost = totalUserGroceryCost
            };

            return View(model);
        }

        public ActionResult JoinMess()
        {
            if (Session["MessId"] != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public ActionResult JoinMess(int messId)
        {
            int userId = (int)Session["UserId"]; 

            if (Session["MessId"] != null)
            {
                return RedirectToAction("Index");
            }

            var messMember = new MessMember
            {
                UserId = userId,
                MessId = messId
            };

            _context.MessMembers.Add(messMember);
            _context.SaveChanges();

            Session["MessId"] = messId;

            return RedirectToAction("Index");
        }

    }
}
