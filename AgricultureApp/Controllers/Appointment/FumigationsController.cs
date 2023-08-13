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
    public class FumigationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Fumigations
        public ActionResult Index(int id)
        {
            return View(db.Fumigations.Where(d=>d.bookingId==id).ToList());
        }

        // GET: Fumigations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fumigation fumigation = db.Fumigations.Find(id);
            if (fumigation == null)
            {
                return HttpNotFound();
            }
            return View(fumigation);
        }

        // GET: Fumigations/Create
        public ActionResult Create(int id, int bookingId, int serviceId)
        {
            var book = db.Bookings.Where(d => d.Id == bookingId).FirstOrDefault();
            var serv = db.FumServices.Where(d => d.Id == serviceId).FirstOrDefault();
            var treat = db.FumTreatments.Where(d => d.Id == id).FirstOrDefault();

            ViewBag.Name = book.CustomerName;
            ViewBag.Service = serv.service;
            ViewBag.Price = treat.Price;
            ViewBag.Treatment = treat.Treatment;
            return View();
        }

        // POST: Fumigations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,width,length")] Fumigation fumigation, int id,int bookingId, int serviceId)
        {
            var book = db.Bookings.Where(d=>d.Id==bookingId).FirstOrDefault();
            var serv = db.FumServices.Where(d => d.Id == serviceId).FirstOrDefault();
            var fum = db.FumTreatments.Find(id);
            if (ModelState.IsValid)
            {
                fumigation.Total = fumigation.width * fumigation.length * fum.Price;
                fumigation.price = fum.Price;
                fumigation.bookingId = bookingId;
                book.isMade = true;
                fumigation.FumService = serv.service;
                fumigation.Treatment = fum.Treatment;
                db.Entry(book).State = EntityState.Modified;
                db.Fumigations.Add(fumigation);
                db.SaveChanges();
                TempData["Index"] = "Your appointment has be submitted, please go to your appointment details and complete payment";
                return RedirectToAction("Index", "Bookings", new {id=bookingId});
            }
            return View(fumigation);
        }

        // GET: Fumigations/Create
        public ActionResult IndoorCreate(int id, int bookingId, int serviceId)
        {
            var book = db.Bookings.Where(d => d.Id == bookingId).FirstOrDefault();
            var serv = db.FumServices.Where(d => d.Id == serviceId).FirstOrDefault();
            var treat = db.FumTreatments.Where(d => d.Id == id).FirstOrDefault();

            ViewBag.Name = book.CustomerName;
            ViewBag.Service = serv.service;
            ViewBag.Price = treat.Price;
            ViewBag.Treatment = treat.Treatment;
            return View();
        }

        // POST: Fumigations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndoorCreate([Bind(Include = "Id,numRooms")] Fumigation fumigation, int id,int bookingId, int serviceId, int numRooms)
        {
            var book = db.Bookings.Where(d=>d.Id==bookingId).FirstOrDefault();
            var serv = db.FumServices.Where(d => d.Id == serviceId).FirstOrDefault();
            var fum = db.FumTreatments.Find(id);
            if (ModelState.IsValid)
            {
                fumigation.NumberOfRooms = numRooms;
                fumigation.Total = numRooms* fum.Price;
                fumigation.price = fum.Price;
                fumigation.bookingId = bookingId;
                book.isMade = true;
                fumigation.FumService = serv.service;
                fumigation.Treatment = fum.Treatment;
                db.Entry(book).State = EntityState.Modified;
                db.Fumigations.Add(fumigation);
                db.SaveChanges();
                TempData["Index"] = "Your appointment has be submitted, please go to your appointment details and complete payment";
                return RedirectToAction("Index", "Bookings", new {id=bookingId});
            }
            return View(fumigation);
        }

        // GET: Fumigations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fumigation fumigation = db.Fumigations.Find(id);
            if (fumigation == null)
            {
                return HttpNotFound();
            }
            return View(fumigation);
        }

        // POST: Fumigations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,bookingId,FumService,Treatment,price,NumberOfRooms,width,length,Total")] Fumigation fumigation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fumigation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fumigation);
        }

        // GET: Fumigations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fumigation fumigation = db.Fumigations.Find(id);
            if (fumigation == null)
            {
                return HttpNotFound();
            }
            return View(fumigation);
        }

        // POST: Fumigations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fumigation fumigation = db.Fumigations.Find(id);
            db.Fumigations.Remove(fumigation);
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
