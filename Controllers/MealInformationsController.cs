using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using messManagement.Models;

namespace messManagement.Controllers
{
    public class MealInformationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private bool IsUserLoggedInAndHasMessId()
        {
            return (Session["UserId"] != null && Session["MessId"] != null) || Session["ManagerId"] != null && Session["MessId"] != null;
        }

        private ActionResult RedirectToHomeIfNotLoggedIn()
        {
            if (!IsUserLoggedInAndHasMessId())
            {
                return RedirectToAction("Index", "Home");
            }
            return null;
        }

        // GET: MealInformations
        public async Task<ActionResult> Index(int? month, int? year)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            int messId = Convert.ToInt32(Session["MessId"]);
            string role = Session["role"]?.ToString();

            if (!month.HasValue) month = DateTime.Now.Month;
            if (!year.HasValue) year = DateTime.Now.Year;

            var meals = db.MealInformations
                .Include(m => m.User)
                .Where(m => m.MessId == messId && m.Month == month && m.Year == year);

            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.role = role; 

            return View(await meals.ToListAsync());
        }

        // GET: MealInformations/Create
        public ActionResult Create()
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            string role = Session["role"]?.ToString();
            if (role == "manager")
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: MealInformations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MealCount,Day,Month,Year")] MealInformation mealInformation)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            string role = Session["role"]?.ToString();
            if (role == "manager")
            {
                return RedirectToAction("Index");
            }

            mealInformation.UserId = Convert.ToInt32(Session["UserId"]);
            mealInformation.MessId = Convert.ToInt32(Session["MessId"]);

            if (ModelState.IsValid)
            {
                db.MealInformations.Add(mealInformation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(mealInformation);
        }

        // GET: MealInformations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MealInformation mealInformation = await db.MealInformations.FindAsync(id);
            if (mealInformation == null)
            {
                return HttpNotFound();
            }

            string role = Session["role"]?.ToString();
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);
            if (role != "manager" && mealInformation.UserId != loggedInUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not allowed to edit this meal.");
            }

            return View(mealInformation);
        }

        // POST: MealInformations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MealCount,Day,Month,Year")] MealInformation mealInformation)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            if (ModelState.IsValid)
            {
                var existingMealInformation = await db.MealInformations.FindAsync(mealInformation.Id);
                if (existingMealInformation == null)
                {
                    return HttpNotFound();
                }

                mealInformation.UserId = existingMealInformation.UserId;

                if (!db.Users.Any(u => u.Id == mealInformation.UserId))
                {
                    ModelState.AddModelError("UserId", "The specified user does not exist.");
                }

                if (ModelState.IsValid)
                {
                    existingMealInformation.MealCount = mealInformation.MealCount;
                    existingMealInformation.Day = mealInformation.Day;
                    existingMealInformation.Month = mealInformation.Month;
                    existingMealInformation.Year = mealInformation.Year;

                    db.Entry(existingMealInformation).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(mealInformation);
        }


        // GET: MealInformations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MealInformation mealInformation = await db.MealInformations.FindAsync(id);
            if (mealInformation == null)
            {
                return HttpNotFound();
            }

            string role = Session["role"]?.ToString();
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);
            if (role != "manager" && mealInformation.UserId != loggedInUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not allowed to delete this meal.");
            }

            return View(mealInformation);
        }

        // POST: MealInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MealInformation mealInformation = await db.MealInformations.FindAsync(id);
            db.MealInformations.Remove(mealInformation);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
