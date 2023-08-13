using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AgricultureApp.Models;
using AgricultureApp.Models.Appointment;
using Microsoft.AspNet.Identity;

namespace AgricultureApp.Controllers.Appointment
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bookings
        [Authorize]
        public ActionResult Index(int id)
        {
            return View(db.Bookings.Where(d => d.Id == id).ToList());
        }

        [Authorize]
        public ActionResult Deliveries()
        {
            var user = User.Identity.GetUserName();
            return View(db.AppDeliveries.Where(d => d.FumEmail == user).ToList());
        }

        [Authorize]
        public ActionResult BookingIndex()
        {
            var user = User.Identity.GetUserName();
            return View(db.Bookings.Where(d => d.CustomerEmail == user && d.isMade == true).ToList());
        }

        public ActionResult BookingInvoice(int id)
        {
            var book = db.Bookings.Find(id);
            ViewBag.Name = book.CustomerName;
            ViewBag.Surname = book.CustomerSurname;
            ViewBag.Email = book.CustomerEmail;
            return View(db.Fumigations.Where(d => d.bookingId == id).ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustomerName,CustomerSurname,CustomerEmail,CustomerContact,CustomerAddress,ServiceType,Date")] Booking booking, DateTime Date)
        {
            if (ModelState.IsValid)
            {
                booking.Date = Date.ToLongDateString();
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("PickIndex", "FumServices", new { id = booking.Id });

            }

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerName,CustomerSurname,CustomerEmail,CustomerContact,CustomerAddress,ServiceType,Date")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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

        public ActionResult Start(int id)
        {
            var del = db.AppDeliveries.Find(id);
            ViewBag.Address = del.Address;
            return View();
        }

		[HttpPost]
        public ActionResult Start(int id, int bookId)
        {
            var del = db.AppDeliveries.Find(id);
            var book = db.Bookings.Where(d => d.Id == bookId).FirstOrDefault();
            del.isPickUp = true;
            book.atDoor = true;

            db.Entry(book).State = EntityState.Modified;
            db.Entry(del).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Deliveries"] = "Arrived, inform the customer to enter their fumigation code to start";
            return RedirectToAction("Deliveries");
        }

        public ActionResult StartFum()
        {
            return View();
        }

		[HttpPost]
        public ActionResult StartFum(string pass, int id)
        {
            var del = db.AppDeliveries.Any(d => d.pass == pass && d.Id==id);
            var deli = db.AppDeliveries.Where(d => d.pass == pass && d.Id==id).FirstOrDefault();

            if (del)
            {
                deli.isConfirmed = true;
                db.Entry(deli).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Deliveries"] = "The client has been confirmed, you may start fumigating.";
                return RedirectToAction("Deliveries");
            }
            else
            {
                TempData["StartFum"] = "Invalid start password, please try again";
                return RedirectToAction("StartFum");
            }
        }


        public ActionResult Finish(int id)
		{
            var b = db.Bookings.Find(id);
           ViewBag.Name= b.CustomerName;
            ViewBag.ID = id;
            return View();
		}

		[HttpPost]
        public ActionResult Finish(int id, string signature)
		{
            var book = db.Bookings.Find(id);
            var del = db.AppDeliveries.Where(d => d.orderId == book.Id).FirstOrDefault();
            var details = db.Fumigations.Where(d => d.bookingId == book.Id).FirstOrDefault();


            details.signauture = Convert.FromBase64String(signature);
            del.isDelivered = true;
            book.isDone = true;
            details.isDone = true;

            db.Entry(del).State = EntityState.Modified;
            db.Entry(details).State = EntityState.Modified;
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
            TempData["DeliveryNote"] = "This appointment has been finished";
            return RedirectToAction("DeliveryNote", new {id=details.Id});
		}


        public ActionResult DeliveryNote(int id)
		{
            var details = db.Fumigations.Where(d => d.Id == id).ToList();
            return View(details);
		}
    }
}
