using Microsoft.AspNet.Identity;
using AgricultureApp.Models.Ratingz;
using AgricultureApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace AgricultureApp.Controllers
{
    public class RatingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ratings
        public ActionResult Index()
        {
            var ratings = db.Ratings;
            return View(ratings.ToList());
        }

        // GET: Ratings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // GET: Ratings/Create
        public ActionResult Create(int? id)
        {
            var user = User.Identity.GetUserName();
            Session["ItemCode"] = id;
            var prop = db.Items.Find(id);

            TempData["GameName"] = prop.name;

            ViewBag.ItemCode = new SelectList(db.Items, "id", "name");
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RatingId,CustomerID,ItemCode,Rank,Comment,OverallRating")] Rating rating, int ratin, int id)
        {
            var user = User.Identity.GetUserName();
            var guest = db.Customers.Where(d => d.Email == user).FirstOrDefault();
            var games = db.Items.Find(id);
            if (ModelState.IsValid)
            {
                rating.GameName = games.name;
                rating.GameId = id;
                rating.CustomerID = guest.Email;
                rating.Rank = ratin;

                db.Ratings.Add(rating);
                db.SaveChanges();

                var game = db.Items.Where(d => d.id == id).FirstOrDefault();

                var c = db.Ratings.Where(d => d.GameId == game.id).Count();


                var sum = db.Ratings.Where(d=>d.GameId==id).Sum(d => d.Rank);
                game.avRating = sum / c;
                game.numOfRate += 1;

                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();

                if (ratin <= 3)
                {
                    var body = $"<p>Hello  {guest.FirstName},</p>" + "<p>Thank you for taking the time to review our store Item. We appreciate your feedback, and we apologize for not meeting your expectations.</p>" +
                        "<p>Your input is invaluable to us, as it helps us improve our products and services. We are actively addressing your concerns with our team to ensure a better experience for our customers in the future.</p>" +
                        "<p>If you have any further questions or feedback, please don't hesitate to reach out to us. We value your satisfaction as a customer, and we are here to assist you.</p>" +
                        "<p>Thank you once again for your review, and we hope to have the opportunity to serve you better in the future.</p>" +
                        "<p>Best Regards,</p>" +
                        "<p>Plantation Management</p>";
                    MailMessage mm = new MailMessage("Plantationgrp33@gmail.com", guest.Email);
                    mm.Subject = "REVIEW REPORT";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    mm.Priority = MailPriority.High;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;

                    NetworkCredential nc = new NetworkCredential("Plantationgrp33@gmail.com", "jkmabstufjzldlsc");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = nc;
                    smtp.Send(mm);
                }
                if (ratin > 3)
                {
                    var body = $"<p>Hello  {guest.FirstName},</p>" + "<p>Thank you for your glowing review of our store Item. We truly appreciate your kind words and support.</p>" +
                       "<p>Your satisfaction is our priority, and we are thrilled that you had a positive experience with store item. We will continue striving to meet and exceed your expectations in the future.</p>" +
                       "<p>If you have any further feedback or need assistance, please don't hesitate to reach out. We value your patronage and look forward to serving you again.</p>" +
                       "<p>Best Regards,</p>" +
                       "<p>Plantation Management</p>";

                    MailMessage mm = new MailMessage("Plantationgrp33@gmail.com", guest.Email);
                    mm.Subject = "REVIEW REPORT";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    mm.Priority = MailPriority.High;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;

                    NetworkCredential nc = new NetworkCredential("Plantationgrp33@gmail.com", "jkmabstufjzldlsc");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = nc;
                    smtp.Send(mm);
                }

                TempData["GameIndex"] = "Thank you for leaving a review";
                return RedirectToAction("Shop", "Carts");
            }

            ViewBag.ItemCode = new SelectList(db.Items, "id", "name", rating.GameId);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", rating.GameId   );
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RatingId,CustomerID,ItemCode,Rank,Comment,OverallRating")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", rating.GameId);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rating rating = db.Ratings.Find(id);
            db.Ratings.Remove(rating);
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