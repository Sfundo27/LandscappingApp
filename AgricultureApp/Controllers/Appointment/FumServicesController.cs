using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AgricultureApp.Models;
using AgricultureApp.Models.Appointment;

namespace AgricultureApp.Controllers.Appointment
{
    public class FumServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FumServices
        public ActionResult Index(int? id=null)
        {
            ViewBag.BookingId = id;
            return View(db.FumServices.ToList());
        }
        // GET: FumServices
        public ActionResult PickIndex(int? id=null)
        {
            ViewBag.BookingId = id;
            return View(db.FumServices.ToList());
        }

        // GET: FumServices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FumService fumService = db.FumServices.Find(id);
            if (fumService == null)
            {
                return HttpNotFound();
            }
            return View(fumService);
        }

        // GET: FumServices/Create
        public ActionResult Create()
        {
          
            return View();
        }

        // POST: FumServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,service,descr")] FumService fumService)
        {
            if (ModelState.IsValid)
            {
                db.FumServices.Add(fumService);
                db.SaveChanges();
                return RedirectToAction("Index", "FumTreatments", new {id=fumService.Id});
            }

            return View(fumService);
        }

        // GET: FumServices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FumService fumService = db.FumServices.Find(id);
            if (fumService == null)
            {
                return HttpNotFound();
            }
            return View(fumService);
        }

        // POST: FumServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,service,descr")] FumService fumService)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fumService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fumService);
        }

        // GET: FumServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FumService fumService = db.FumServices.Find(id);
            if (fumService == null)
            {
                return HttpNotFound();
            }
            return View(fumService);
        }

        // POST: FumServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FumService fumService = db.FumServices.Find(id);
            db.FumServices.Remove(fumService);
            db.SaveChanges();
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
