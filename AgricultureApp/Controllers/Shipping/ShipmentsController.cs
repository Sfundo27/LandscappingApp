using Microsoft.AspNet.Identity;
using AgricultureApp.Models;
using AgricultureApp.Models.Shipping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AgricultureApp.Models.Product;
using System.Net.Mail;

namespace AgricultureApp.Controllers.Shipping
{
    public class ShipmentsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
		{
			if (User.IsInRole("Driver"))
			{
                var user = User.Identity.GetUserName();
                var driver = db.Drivers.Where(c => c.Email == user).FirstOrDefault();
                return View(db.Shipments.ToList().Where(c=>c.DriverEmail==driver.Email&&c.isDelivered==false));
			}
            return View(db.Shipments.ToList());
		}

        public ActionResult GetDirections(int orderId)
		{
            var o = db.Orders.Where(d => d.Id == orderId).ToList();
            return View(o);
		}

		[HttpPost]
        public ActionResult GetDirections(int id, string duration, string Destination, string Starting)
		{
            var o = db.Orders.Where(d => d.Id == id).FirstOrDefault();

			var body = $"Hello  {o.CustomerName}," + "<br/><br/>" +  "Your order has now left the store, please expect it to arrive in " + duration + "<br/>" + "The delivery guy is driving from " + Starting;
			MailMessage mm = new MailMessage("Plantationgrp33@gmail.com", o.CustomerEmail);
			mm.Subject = "DELIVERY REPORT | Order #:" + o.OrderNo;
			mm.Body = body + "<br/><br/>" + "Thank you for shopping at our store." + "<br/>" +
			"Hoticulture Management";
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
			TempData["Start"] = "The customer has recieved an alert of your departure, NOTE: use these directions to reach to "+Destination;
			return RedirectToAction("Start", new {orderId=o.Id, location=Destination, starting=Starting});
		}

        public ActionResult Start(int orderId, string location, string starting)
		{
            var o = db.Orders.Where(d => d.Id == orderId).FirstOrDefault();
            var order = db.Orders.Where(d => d.Id == orderId).ToList();
			ViewBag.Location = location;
			ViewBag.Starting = starting;
			ViewBag.Name = o.CustomerName;
			return View(order);
		}

		[HttpPost]
        public ActionResult Start(int orderId)
		{
			var o = db.Orders.Where(d => d.Id == orderId).FirstOrDefault();
			var del = db.Shipments.Where(d => d.OrderId == o.Id).FirstOrDefault();
			var body = $"Hello  {o.CustomerName}," + "\n" + "\n" + "Your parcel has arrived, Please meet our driver at the gate " + "\n" + "\n" + "Kind Regards.";
			MailMessage mm = new MailMessage("Plantationgrp33@gmail.com", o.CustomerEmail);
			mm.Subject = "PARCEL ARRIVAL | Order #:" + o.OrderNo;
			mm.Body = body + "\n" +
					   "Hoticulture Management";
			mm.IsBodyHtml = false;
			mm.Priority = MailPriority.High;

			SmtpClient smtp = new SmtpClient();
			smtp.Host = "smtp.gmail.com";
			smtp.Port = 587;
			smtp.EnableSsl = true;

			NetworkCredential nc = new NetworkCredential("Plantationgrp33@gmail.com", "jkmabstufjzldlsc");
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = nc;
			smtp.Send(mm);
			o.isPickUp = true;
			del.isPickUp = true;
			db.Entry(o).State = EntityState.Modified;
			db.Entry(del).State = EntityState.Modified;
			db.SaveChanges();
			TempData["Index"] = "The customer has recieved an alert of your arrival, please be patient";
			return RedirectToAction("Index");
		}

		public ActionResult Confirm(int orderId)
		{
			var o = db.Orders.Where(d => d.Id == orderId).FirstOrDefault();
			ViewBag.ID = o.Id;
			return View();
		}

		[HttpPost]
		public ActionResult Confirm(int orderId, string signature)
		{
			var o = db.Orders.Where(d => d.Id == orderId).FirstOrDefault();
			var del = db.Shipments.Where(d => d.OrderId == o.Id).FirstOrDefault();

			o.signature = Convert.FromBase64String(signature);
			del.isDelivered = true;
			o.isDelivered = true;

			var callbackUrl = Url.Action("Invoice", "Shipments", new { id = o.Id }, protocol: Request.Url.Scheme);
			var body = $"Hello  {o.CustomerName}," + "\n" + "\n" + "You have accepted your parcel, Please see your order report <a href=\"" + callbackUrl + "\">here!<a/> " + "\n" + "\n" + "Thank you for shopping with us.";
			MailMessage mm = new MailMessage("Plantationgrp33@gmail.com", o.CustomerEmail);
			mm.Subject = "PARCEL RECEIVED | Order #:" + o.OrderNo;
			mm.Body = body + "\n" +
					   "Hoticulture Management";
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

			db.Entry(del).State = EntityState.Modified;
			db.Entry(o).State = EntityState.Modified;
			db.SaveChanges();
			TempData["Index"] = "DELIVERY CONFIRMED, Tell the customer to check their emails";
			return RedirectToAction("Index");
		}


		public ActionResult Invoice(int id)
		{
			var order = db.Orders.Where(d => d.Id == id).SingleOrDefault();
			var orderItems = db.OrderItems.Where(d => d.OrderId == id).ToList();

			var viewModel = new OrderViewModel
			{
				Order = order,
				OrderItems = orderItems
			};

			return View(viewModel);
		}

	}
}