using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AgricultureApp.Models;
using AgricultureApp.Models.Chat;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;


namespace AgricultureApp.Controllers.Chat
{
    public class ChatBoxesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult UserIndex()
        {
            var user = User.Identity.GetUserName();
            var chat = db.Chatters.FirstOrDefault(d => d.Email == user);

            if (chat == null)
            {
                var customer = db.Customers.FirstOrDefault(d => d.Email == user);
                if (customer != null)
                {
                    Chatter newChat = new Chatter
                    {
                        Email = customer.Email,
                        Name = customer.FirstName,
                        Surname = customer.LastName
                    };

                    db.Chatters.Add(newChat);
                    db.SaveChanges();

                    chat = newChat;
                }
            }
            return View(db.ChatBoxes.Where(d => d.chatterId == chat.Id).ToList());
        }

        public ActionResult AdminIndex(int? id)
        {
			if (id != null)
			{
                var chat = db.Chatters.Where(d => d.Id == id).FirstOrDefault();
                var box = db.ChatBoxes.ToList();
                var chatIds = box.Select(i => i.chatterId).Distinct().ToList();
                ViewBag.Chat = db.Chatters.Where(d => chatIds.Contains(d.Id)).ToList();
                ViewBag.NumChat = db.ChatBoxes.Where(d => d.Id == id).Count();
                ViewBag.Id = id;
                ViewBag.Name = chat.Name;
                return View(db.ChatBoxes.Where(d => d.chatterId == chat.Id).ToList());
            }
			else
			{
                var box = db.ChatBoxes.ToList();
                var chatIds = box.Select(i => i.chatterId).Distinct().ToList();
                ViewBag.Chat = db.Chatters.Where(d => chatIds.Contains(d.Id)).ToList();
                return View(db.ChatBoxes.Where(d => d.chatterId == id).ToList());
            }
        }

        public ActionResult Create([Bind(Include = "Id,message")] ChatBox chatBox)
        {
            if (ModelState.IsValid)
            {
                
                var user = User.Identity.GetUserName();
                var chat = db.Chatters.FirstOrDefault(d => d.Email == user);
				if (chat == null)
				{
                    var customer = db.Customers.Where(d => d.Email == user).FirstOrDefault();
                    Chatter chats = new Chatter
                    {
                        Email = customer.Email,
                        Name = customer.FirstName,
                        Surname = customer.LastName
                    };
                    db.Chatters.Add(chats);
                    db.SaveChanges();
                }

                chatBox.chatterId = chat.Id;
                chatBox.time = DateTime.Now.ToLongTimeString();
                chatBox.date = DateTime.Now.ToLongDateString();
                chatBox.sender = user;
                chatBox.reciever = "Admin@Agric.com";
                db.ChatBoxes.Add(chatBox);
                db.SaveChanges();

                // Return a JSON response indicating success
                return Json(new { success = true , message = chatBox.message });
            }

            // Return a JSON response indicating failure with error messages
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }
        public ActionResult AdminCreate([Bind(Include = "Id,message")] ChatBox chatBox, int id)
        {
            if (ModelState.IsValid)
            {

                var chat = db.Chatters.Find(id);
                chatBox.sender = "Admin@Agric.com";
                chatBox.reciever = chat.Email;
                chatBox.chatterId = chat.Id;
                chatBox.time = DateTime.Now.ToLongTimeString();
                chatBox.date = DateTime.Now.ToLongDateString();
                db.ChatBoxes.Add(chatBox);
                db.SaveChanges();

                // Return a JSON response indicating success
                return Json(new { success = true , message = chatBox.message });
            }

            // Return a JSON response indicating failure with error messages
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }

    }
}
