using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using messManagement.Models;

namespace messManagement.Controllers
{
    public class UtilityBillsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UtilityBills
        public async Task<ActionResult> Index()
        {
            int? messId = Session["MessId"] as int?;

            if (messId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var utilityBills = await db.UtilityBills
                .Where(u => u.MessId == messId && u.Month == currentMonth && u.Year == currentYear)
                .Include(u => u.Mess)
                .ToListAsync();

            var totalUtilityCost = utilityBills.Sum(u => u.UtilityCost);

            ViewBag.TotalUtilityCost = totalUtilityCost;

            return View(utilityBills);
        }


        // GET: UtilityBills/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UtilityBill utilityBill = await db.UtilityBills.FindAsync(id);
            if (utilityBill == null)
            {
                return HttpNotFound();
            }

            return View(utilityBill);
        }


        // GET: UtilityBills/Create
        public ActionResult Create()
        {
            if (Session["role"] != null && Session["role"].ToString() == "manager")
            {
                int messId = (int)Session["MessId"];
                var mess = db.Messes.Find(messId);

                ViewBag.MessName = mess?.MessName;

                UtilityBill utilityBill = new UtilityBill { MessId = messId };

                return View(utilityBill);
            }
            return RedirectToAction("Index");
        }

        // POST: UtilityBills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UtilityName,UtilityCost,Month,Year")] UtilityBill utilityBill)
        {
            if (Session["role"] != null && Session["role"].ToString() == "manager")
            {
                utilityBill.MessId = (int)Session["MessId"];

                if (ModelState.IsValid)
                {
                    db.UtilityBills.Add(utilityBill);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                var mess = db.Messes.Find(utilityBill.MessId);
                ViewBag.MessName = mess?.MessName;
            }
            return RedirectToAction("Index");
        }


        // GET: UtilityBills/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["role"] == null || Session["role"].ToString() != "manager")
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UtilityBill utilityBill = await db.UtilityBills.FindAsync(id);
            if (utilityBill == null)
            {
                return HttpNotFound();
            }

            ViewBag.MessId = new SelectList(db.Messes, "Id", "MessName", utilityBill.MessId);
            return View(utilityBill);
        }

        // POST: UtilityBills/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MessId,UtilityName,UtilityCost,Month,Year")] UtilityBill utilityBill)
        {
            if (Session["role"] != null && Session["role"].ToString() == "manager")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(utilityBill).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.MessId = new SelectList(db.Messes, "Id", "MessName", utilityBill.MessId);
                return View(utilityBill);
            }
            return RedirectToAction("Index");
        }

        // GET: UtilityBills/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (Session["role"] == null || Session["role"].ToString() != "manager")
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UtilityBill utilityBill = await db.UtilityBills.FindAsync(id);
            if (utilityBill == null)
            {
                return HttpNotFound();
            }

            return View(utilityBill);
        }

        // POST: UtilityBills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (Session["role"] == null || Session["role"].ToString() != "manager")
            {
                return RedirectToAction("Index");
            }

            UtilityBill utilityBill = await db.UtilityBills.FindAsync(id);
            db.UtilityBills.Remove(utilityBill);
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
