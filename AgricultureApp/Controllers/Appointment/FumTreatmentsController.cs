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
    public class FumTreatmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FumTreatments
        public ActionResult Index(int? id=null, int? bookingId=null)
        {
            ViewBag.ServiceId = id;
            ViewBag.bookingId = bookingId;
            return View(db.FumTreatments.Where(d=>d.ServiceId== id).ToList());
        }
        public ActionResult PickIndex(int? id=null, int? bookingId=null)
        {
            ViewBag.ServiceId = id;
            ViewBag.bookingId = bookingId;
            return View(db.FumTreatments.Where(d=>d.ServiceId== id).ToList());
        }

        // GET: FumTreatments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FumTreatment fumTreatment = db.FumTreatments.Find(id);
            if (fumTreatment == null)
            {
                return HttpNotFound();
            }
            return View(fumTreatment);
        }

        // GET: FumTreatments/Create
        public ActionResult Create(int id)
        {
            return View();
        }

        // POST: FumTreatments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ServiceId,Treatment,Price")] FumTreatment fumTreatment, int id)
        {
            if (ModelState.IsValid)
            {
                fumTreatment.ServiceId = id;
                db.FumTreatments.Add(fumTreatment);
                db.SaveChanges();
                return RedirectToAction("Index", new {id=fumTreatment.ServiceId});
            }
            return View(fumTreatment);
        }

        // GET: FumTreatments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FumTreatment fumTreatment = db.FumTreatments.Find(id);
            if (fumTreatment == null)
            {
                return HttpNotFound();
            }
            return View(fumTreatment);
        }

        // POST: FumTreatments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ServiceId,Treatment,Price")] FumTreatment fumTreatment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fumTreatment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fumTreatment);
        }

        // GET: FumTreatments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FumTreatment fumTreatment = db.FumTreatments.Find(id);
            if (fumTreatment == null)
            {
                return HttpNotFound();
            }
            return View(fumTreatment);
        }

        // POST: FumTreatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FumTreatment fumTreatment = db.FumTreatments.Find(id);
            db.FumTreatments.Remove(fumTreatment);
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
