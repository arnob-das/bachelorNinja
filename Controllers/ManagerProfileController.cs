using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using messManagement.Models;

namespace messManagement.Controllers
{
    public class ManagerProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManagerProfile
        public async Task<ActionResult> Index()
        {
            return View(await db.Managers.ToListAsync());
        }

        // GET: ManagerProfile/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                if (Session["ManagerId"] == null)
                {
                    return RedirectToAction("ManagerLogin", "Login");
                }
                id = Convert.ToInt32(Session["ManagerId"]);
            }

            Manager manager = await db.Managers.FindAsync(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        // GET: ManagerProfile/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagerProfile/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FullName,PhoneNo,NidNo,Email,Password")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                db.Managers.Add(manager);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(manager);
        }

        // GET: ManagerProfile/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = await db.Managers.FindAsync(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        // POST: ManagerProfile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FullName,PhoneNo,NidNo,Email,Password")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manager).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(manager);
        }

        // GET: ManagerProfile/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = await db.Managers.FindAsync(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }
            
        // POST: ManagerProfile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Manager manager = await db.Managers.FindAsync(id);
            db.Managers.Remove(manager);
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
