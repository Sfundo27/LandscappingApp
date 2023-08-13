using Microsoft.AspNet.Identity;
using AgricultureApp.Models;
using AgricultureApp.Models.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgricultureApp.Controllers.Product
{
    public class CartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Shop(int? id)
        {
            var items_results = new List<Item>();

            try
            {
                if (id != null)
                {
                    if (id == 0)
                    {
                        items_results = db.Items.ToList();
                        ViewBag.Department = "All Departments";
                    }
                    else
                    {
                        items_results = db.Items.Where(x => x.catID == (int)id).ToList();
                        var cat = db.Categories.Find(id);
                        ViewBag.Department = cat.name;
                    }
                }
                else
                {
                    items_results = db.Items.ToList();
                    ViewBag.Department = "All Departments";
                }
            }
            catch (Exception ex) { }
            return View(items_results);
        }

        [Authorize]
        public ActionResult AddToCart(int id)
        {
            var item = db.Items.Where(d => d.id == id).FirstOrDefault();

            // Get the current user's ID, or null if the user is not authenticated
            string userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;

            // Try to find an existing cart for the user
            var cart = db.Carts.SingleOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                // If no cart exists, create a new one for the user
                cart = new Cart { UserId = userId };
                db.Carts.Add(cart);
            }

            // Try to find an existing cart item for the product
            var cartItem = db.CartItems.Where(d => d.userID == cart.UserId).FirstOrDefault(ci => ci.itemID == id);

            if (cartItem == null)
            {
                // If no cart item exists, create a new one
                cartItem = new CartItem { itemID = id, Quantity = 1, Product = item.name, userID = cart.UserId, price = item.price };
                db.CartItems.Add(cartItem);
            }
            else
            {
                // If a cart item exists, update its quantity
                cartItem.Quantity += 1;
            }

            db.SaveChanges();

            return RedirectToAction("Shop");
        }

        public ActionResult UpdateCart(int id, int quantity)
        {
            var cart = db.CartItems.Where(d => d.Id == id);
            var cartItem = cart.FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                db.Entry(cartItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetMyCart", "Carts");
            }
            else
            {
                return Json(new { success = false, message = "Item not found in cart" });
            }
        }


        public ActionResult RemoveItem(int? id)
        {

            if (id != null)
            {
                var cartItem = db.CartItems.Find(id);
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
                return RedirectToAction("GetMyCart", "Carts");
            }
            else
            {
                return Json(new { success = false, message = "Item not found in cart." });
            }
        }


        public ActionResult GetMyCart()
        {
            var userId = User.Identity.GetUserId();
            var cart = db.Carts.Where(d => d.UserId == userId).FirstOrDefault();
            var cartItem = db.CartItems.Where(d => d.userID == userId).ToList();
            double cartTotal = 0;
            if (cartItem != null && cartItem.Any())
            {
                foreach (var i in cartItem)
                {
                    cartTotal += (i.Quantity * i.price);
                }
            }
            ViewBag.Total = cartTotal;
            return View(cartItem);
        }

        public ActionResult ClearCart()
        {
            // Delete all items from the cart
            db.CartItems.RemoveRange(db.CartItems);
            db.SaveChanges();

            // Redirect the user back to the cart view
            return RedirectToAction("GetMyCart");
        }



        public ActionResult Checkout()
        {
            var userId = User.Identity.GetUserId();
            var cart = db.Carts.SingleOrDefault(c => c.UserId == userId);
            int? id = null;
            var user = User.Identity.GetUserName();
            var guest = db.Customers.Where(d => d.Email == user).FirstOrDefault();

            if (cart == null)
            {
                return RedirectToAction("GetMyCarts", "Carts");
            }

            var cartItems = db.CartItems.Where(ci => ci.userID == cart.UserId).ToList();


            double totalAmount = 0;

            Order o = new Order();
            o.userId = userId;
            o.CustomerName = guest.FirstName + " " + guest.LastName;
            o.CustomerEmail = guest.Email;
            o.OrderDate = DateTime.Now.ToLongDateString();
            o.OrderStatus = "Ordered, please update your address";
            o.PaymentStatus = "Payment due";
            foreach (var i in cartItems)
            {
                totalAmount += i.Quantity * i.Item.price;
                o.Total = totalAmount;

                OrderItem orderItems = new OrderItem();
                orderItems.ProductId = i.itemID;
                orderItems.itemName = i.Product;
                orderItems.Quantity = i.Quantity;
                orderItems.UnitPrice = i.Item.price;

                db.OrderItems.Add(orderItems);
                db.CartItems.Remove(i);
            }

            db.Orders.Add(o);
            db.SaveChanges();
            id = o.Id;
            return RedirectToAction("Confirmation", "Carts", new { id = id });
        }

        public ActionResult Confirmation(int id)
        {
            return View(db.Orders.Where(d => d.Id == id).ToList());
        }
    }

}