using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using messManagement.Models;

namespace messManagement.Controllers
{
    public class GroceryCostsController : Controller
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

        // GET: GroceryCosts
        public async Task<ActionResult> Index(int? month, int? year)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            int messId = Convert.ToInt32(Session["MessId"]);
            string role = Session["role"]?.ToString();

            if (!month.HasValue) month = DateTime.Now.Month;
            if (!year.HasValue) year = DateTime.Now.Year;

            var costs = db.GroceryCosts
                .Include(g => g.User)
                .Where(g => g.MessId == messId && g.Month == month && g.Year == year);

            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.Role = role;

            return View(await costs.ToListAsync());
        }

        // GET: GroceryCosts/Create
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Day,Month,Year,Amount")] GroceryCost groceryCost)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            string role = Session["role"]?.ToString();
            if (role == "manager")
            {
                return RedirectToAction("Index");
            }

            groceryCost.UserId = Convert.ToInt32(Session["UserId"]);
            groceryCost.MessId = Convert.ToInt32(Session["MessId"]);

            if (ModelState.IsValid)
            {
                db.GroceryCosts.Add(groceryCost); 
                await db.SaveChangesAsync(); 
                return RedirectToAction("Index"); 
            }

            return View(groceryCost);
        }



        // GET: GroceryCosts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GroceryCost groceryCost = await db.GroceryCosts.FindAsync(id);
            if (groceryCost == null)
            {
                return HttpNotFound();
            }

            ViewBag.MessList = db.Messes.ToList();

            string role = Session["role"]?.ToString();
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);
            if (role != "manager" && groceryCost.UserId != loggedInUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not allowed to edit this grocery cost.");
            }

            return View(groceryCost);
        }

        // POST: GroceryCosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(GroceryCost groceryCost)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            var existingGroceryCost = await db.GroceryCosts.FindAsync(groceryCost.Id);
            if (existingGroceryCost == null)
            {
                return HttpNotFound();
            }

            groceryCost.MessId = Convert.ToInt32(Session["MessId"]);

            if (!db.Messes.Any(m => m.Id == groceryCost.MessId))
            {
                ModelState.AddModelError("MessId", "The selected Mess does not exist.");
            }

            groceryCost.UserId = existingGroceryCost.UserId;

            if (ModelState.IsValid)
            {
                existingGroceryCost.Day = groceryCost.Day;
                existingGroceryCost.Month = groceryCost.Month;
                existingGroceryCost.Year = groceryCost.Year;
                existingGroceryCost.Amount = groceryCost.Amount;

                db.Entry(existingGroceryCost).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MessList = db.Messes.ToList();

            return View(groceryCost);
        }






        // GET: GroceryCosts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var redirect = RedirectToHomeIfNotLoggedIn();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroceryCost groceryCost = await db.GroceryCosts.FindAsync(id);
            if (groceryCost == null)
            {
                return HttpNotFound();
            }

            string role = Session["role"]?.ToString();
            int loggedInUserId = Convert.ToInt32(Session["UserId"]);
            if (role != "manager" && groceryCost.UserId != loggedInUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not allowed to delete this grocery cost.");
            }

            return View(groceryCost);
        }

        // POST: GroceryCosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            GroceryCost groceryCost = await db.GroceryCosts.FindAsync(id);
            if (groceryCost != null)
            {
                db.GroceryCosts.Remove(groceryCost);
                await db.SaveChangesAsync();
            }
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
