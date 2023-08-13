using Microsoft.AspNet.Identity;
using AgricultureApp.Models;
using AgricultureApp.Models.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using AgricultureApp.Models.MyBill;

namespace AgricultureApp.Controllers.Product
{
    public class OrdersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Orders
        public ActionResult Details(int? id)
        {
            var del = db.Deliveries.Where(c => c.orderId == id).FirstOrDefault();
            if (del != null)
            {
                ViewBag.DelID = del.Id;
            }

            List<Order> qs = new List<Order>();
            List<OrderItem> ans = new List<OrderItem>();

            if (id != null)
            {

                var q = db.OrderItems.Where(d => d.OrderId == id).ToList();
                foreach (var qItem in q)
                {
                    ans.Add(qItem);
                }

                var a = db.Orders.ToList().Where(d => d.Id == id);
                foreach (var qItem in a)
                {
                    qs.Add(qItem);
                }
            }
            OrderItem r = new OrderItem();
            r.orders = qs;
            r.orderItems = ans;
            ViewBag.id = id;
            return View(r);
        }

        public ActionResult Index(int id)
        {
            var user = User.Identity.GetUserName();
            return View(db.Orders.Where(d => d.CustomerEmail == user&&d.Id==id).ToList());
        }
        public ActionResult MyIndex()
        {
            var user = User.Identity.GetUserName();
            return View(db.Orders.Where(d => d.CustomerEmail == user).ToList());
        }

        public ActionResult ItemsIndex(int id)
        {
            var order = db.Orders.Find(id);
            ViewBag.Total = order.Total;
            return View(db.OrderItems.Where(d => d.OrderId == id).ToList());
        }


        public ActionResult Update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order food = db.Orders.Find(id);
            ViewBag.id = id;
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int? id, Order foodOrder)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);

            if (order != null)
            {
                order.DeliveryAddress = foodOrder.DeliveryAddress;
                order.OrderStatus = "Order Requested, Please proceed to payment";
                order.isUpdated = true;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success123"] = "Updated Successfully.";
                return RedirectToAction("Index", new {id});
            }
            return View(order);
        }


        public ActionResult Add_To_Bill(double TotalPrice, int id)
        {
            var user = User.Identity.GetUserName();
            var guestbill = db.Customers.Where(x => x.Email == user).FirstOrDefault();
            double amount = TotalPrice;

            OrderBill bill = new OrderBill();
            bill = db.OrderBills.Where(b => b.Email == guestbill.Email).FirstOrDefault();


            Order game = new Order();
            game = db.Orders.Where(x => x.Id==id).FirstOrDefault();


            game.OrderStatus = "Billed";
            game.isBilled = true;
            bill.OrderAmount += game.Total;
            bill.TotalAmount += bill.OrderAmount;
            bill.Status = "You have R " + bill.TotalAmount + " amount due";
            bill.itemCode = game.Id;
            db.Entry(bill).State = EntityState.Modified;
            db.Entry(game).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("GetOrderMyBill", "Home");
        }



        public ActionResult AllOrders()
        {
            return View(db.Orders.ToList().Where(x => x.OrderStatus != "Paid"));
        }

        public ActionResult OrderRequests()
        {
            return View(db.Orders.ToList().Where(x => x.OrderStatus != "Accepted"));
        }

        public ActionResult Confirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order != null)
            {
                order.OrderStatus = "Confirmed";
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AllOrders");
            }
            return View(order);
        }


        public ActionResult Process(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(int? id, Order foodOrder)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            var user = User.Identity.GetUserName();
            var assist = db.Assistants.Where(a => a.Email == user).FirstOrDefault();


            if (order != null)
            {
                order.OrderStatus = "Order Processed, Please wait while we prepare shipment";
                order.AssistanceEmail = assist.Email;
                order.AssistanceName = assist.FirstName;
                order.ApproveDate = DateTime.Now.ToLongDateString();
                order.ApproveTime = DateTime.Now.ToShortTimeString();
                order.expDelDate = foodOrder.expDelDate;
                order.addressValid = foodOrder.addressValid;
                if (order.addressValid == "Invalid address")
                {
                    order.OrderStatus = "Invalid address, please provide a correct one";
                }
                else
                {
                    order.OrderStatus = "Order Processed, Please wait while we prepare shipment";
                }
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success123"] = "Order has been processed, the customer is notified.";
                return RedirectToAction("AllOrders");
            }
            return View(order);
        }



        public ActionResult Ship(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ship(int? id, Order game)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            var user = User.Identity.GetUserName();
            var assist = db.Drivers.Where(a => a.Email == user).FirstOrDefault();


            if (order != null)
            {
                order.OrderStatus = "Parcel on the way";
                order.shipmentStatus = "Shipped by :";
                order.dEmail = assist.Email;
                order.dName = assist.FirstName;
                order.dVehicle = assist.Vehicle;
                order.carPlate = assist.CarRegNo;
                order.ShipDate = DateTime.Now.ToLongDateString();
                order.ShipTime = DateTime.Now.ToShortTimeString();
                order.expDelDate = game.expDelDate;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success123"] = "Parcel is on the way, the customer has been notified.";
                return RedirectToAction("AllOrders");
            }
            return View(order);
        }



        public ActionResult Arrive(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Arrive(int? id, Order game)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            var user = User.Identity.GetUserName();
            var assist = db.Drivers.Where(a => a.Email == user).FirstOrDefault();


            if (order != null)
            {
                order.OrderStatus = "Arrived";
                order.shipmentStatus = "Delivered by :";
                order.dEmail = assist.Email;
                order.dName = assist.FirstName;
                order.dVehicle = assist.Vehicle;
                order.carPlate = assist.CarRegNo;
                order.ArrivalDate = DateTime.Now.ToLongDateString();
                order.ArrivalTime = DateTime.Now.ToShortTimeString();
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success123"] = "Parcel delivered, the customer has been notified.";
                return RedirectToAction("AllOrders");
            }
            return View(order);
        }



        public ActionResult Receive(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Receive(int? id, Order game)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Order order = db.Orders.Find(id);
            var user = User.Identity.GetUserName();
            var assist = db.Customers.Where(a => a.Email == user).FirstOrDefault();


            if (order != null)
            {
                order.OrderStatus = "Received";
                order.shipmentStatus = "Delivered by :";
                order.CustomerEmail = assist.Email;
                order.CustomerName = assist.FirstName;
                order.receiveDate = DateTime.Now.ToLongDateString();
                order.receiveTime = DateTime.Now.ToShortTimeString();
                order.EarlyLateParcel = game.EarlyLateParcel;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success123"] = "Parcel received, the store has been notified.";
                return RedirectToAction("Index", new { id });
            }

            return View(order);
        }

        public ActionResult AcceptOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order food = db.Orders.Find(id);
            if (food == null)
            {
                return HttpNotFound();
            }
            return View(food);
        }

        public ActionResult OrderHistory(int id)
        {
            return View(db.Orders.Where(d => d.Id == id && d.isDelivered == true));
        }
    }
}