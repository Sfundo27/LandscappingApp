using AgricultureApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgricultureApp.Controllers
{
	public class HomeController : Controller
	{
		ApplicationDbContext db = new ApplicationDbContext();
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}


		public ActionResult ThankYou(int id)
		{
			ViewBag.id = id;
			return View();
		}

		public ActionResult GetOrderMyBill()
		{
			var user = User.Identity.GetUserName();
			var customer = db.Customers.Where(g => g.Email == user).FirstOrDefault();
			var bill = db.OrderBills.Where(b => b.Email == customer.Email);
			return View(bill);
		}
	}
}