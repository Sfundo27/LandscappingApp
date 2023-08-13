using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayFast;
using PayFast.AspNet;
using AgricultureApp.Models;
using System.Configuration;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using AgricultureApp.Models.Product;
using System.Security.Cryptography;
using AgricultureApp.Models.MyBill;
using AgricultureApp.Models.Shipping;

namespace AgricultureApp.Controllers.Payment
{
    public class PaymentBookController : Controller
    {
        // GET: PaymentBook
        private ApplicationDbContext db = new ApplicationDbContext();
        //Cart_Service cartService = new Cart_Service();
        string UrlLink = "https://2023grp33.azurewebsites.net";

        public PaymentBookController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }

        //Payment
        #region Fields

        private readonly PayFastSettings payFastSettings;

        #endregion Fields

        #region Constructor

        //public ApprovedOwnersController()
        //{

        //}

        #endregion Constructor

        #region Methods


        public ActionResult Recurring()
        {
            var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            recurringRequest.merchant_id = this.payFastSettings.MerchantId;
            recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
            recurringRequest.return_url = this.payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
            recurringRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            recurringRequest.email_address = "loongohthabethe@gmail.com";

            // Transaction Details
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Hotel Reservation";

            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "Paramounthotel23@gmail.com";

            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.Now;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult OnceOff(double TotalPrice, int id, int bookId)
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            //string BillId = Session["billId"].ToString();


            var gameOrder = db.Fumigations.Where(b => b.Id == id).FirstOrDefault();
            var book = db.Bookings.Where(b => b.Id == bookId).FirstOrDefault();
            gameOrder.isPaid = true;



            // Get a list of all drivers from the delivery table
            // Get a list of all drivers from the drivers table
            var drivers = db.Assistants.ToList();

            // Count the number of deliveries for each driver
            var driverTasks = db.AppDeliveries.GroupBy(d => d.FumEmail)
                                           .Select(g => new { DriverEmail = g.Key, TaskCount = g.Count() })
                                           .ToList();

            // Order drivers by the number of deliveries they have, in ascending order
            var orderedDrivers = drivers.OrderBy(d => driverTasks.FirstOrDefault(t => t.DriverEmail == d.Email)?.TaskCount ?? 0);

            // Select the driver who appears the least in the delivery table
            var selectedDriver = orderedDrivers.FirstOrDefault();

            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+={}[]\\|/?<>,.";
            byte[] randomBytes = new byte[8];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            char[] chars = new char[8];
            for (int i = 0; i < 8; i++)
            {
                int index = randomBytes[i] % validChars.Length;
                chars[i] = validChars[index];

            }

            var charsz = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = charsz[random.Next(charsz.Length)];
            }

            var finalString = new String(stringChars);

            // Create a new delivery object and add it to the database
            var newDelivery = new AppDelivery
            {
                FumEmail = selectedDriver.Email,
                FumName = selectedDriver.FirstName + " " + selectedDriver.LastName,
                Address = book.CustomerAddress,
                Name = book.CustomerName,
                DeliveryDate = DateTime.Now.ToLongDateString(),
                orderId = bookId,
                pass = finalString
            };
            db.Entry(gameOrder).State = EntityState.Modified;
            db.AppDeliveries.Add(newDelivery);
            db.SaveChanges();

            double TotalAmount = TotalPrice;

            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = UrlLink + "/Bookings/BookingInvoice?id=" + gameOrder.bookingId;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details

            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            //onceOffRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = TotalAmount;
            onceOffRequest.item_name = "Fumigation services";
            onceOffRequest.item_description = "Fumigation Appointment Payment";

            // BusinessLogic.UpdateRoomsAvailable(roomBooking.RoomId);
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            //cartService.EmptyCart();

            var invoice = Url.Action("Home", "Invoice", new { id }, protocol: Request.Url.Scheme);
            var proof = Url.Action("Home", "Proof", new { id }, protocol: Request.Url.Scheme);

            MailMessage mm = new MailMessage("Plantationgrp33@gmail.com", book.CustomerEmail);
            mm.Subject = "CONFIRMATION CODE";
            mm.Body = "Dear " + book.CustomerName + "," + "<br/><br/>" +
                      "Please see your booking access code below:" + "<br/><br/>" +
                      "<b style=\"color: green; font-size:20px;\">-" +finalString+"</b><br/><br/>" +
                      "Use this code when confirming you identity" + "<br/><br/>" +
                      "Best regards," + "<br/>" +
                      "Plantation Management";

            mm.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("Plantationgrp33@gmail.com", "jkmabstufjzldlsc");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mm);

            TempData["BookingInvoice"] = "Thank you for making payment  please check your emails and NOTE: "+selectedDriver.FirstName+" "+selectedDriver.LastName+" has been selected as your fumigator, please wait so they start their journey to you";
            return Redirect(redirectUrl);
        }

        public ActionResult OrderPayment(double TotalPrice, int id)
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            //string BillId = Session["billId"].ToString();

            var userName = User.Identity.GetUserName();
            var guest = db.Customers.Where(x => x.Email == userName).FirstOrDefault();

            OrderBill mybill = new OrderBill();
            mybill = db.OrderBills.Where(b => b.Email == guest.Email).FirstOrDefault();
            mybill.Status = "No funds are due at the moment!";

            mybill.OrderAmount = 0;
            mybill.TotalAmount = 0;

            Order gameOrder = new Order();
            gameOrder = db.Orders.Where(b => b.Id==id).FirstOrDefault();
            gameOrder.OrderStatus = "Waiting for delivery preparation, please be patient";
            gameOrder.PaymentStatus = "Order settled";
            gameOrder.isPaid = true;



            // Get a list of all drivers from the delivery table
            // Get a list of all drivers from the drivers table
            var drivers = db.Drivers.ToList();

            // Count the number of deliveries for each driver
            var driverTasks = db.Shipments.GroupBy(d => d.DriverEmail)
                                           .Select(g => new { DriverEmail = g.Key, TaskCount = g.Count() })
                                           .ToList();

            // Order drivers by the number of deliveries they have, in ascending order
            var orderedDrivers = drivers.OrderBy(d => driverTasks.FirstOrDefault(t => t.DriverEmail == d.Email)?.TaskCount ?? 0);

            // Select the driver who appears the least in the delivery table
            var selectedDriver = orderedDrivers.FirstOrDefault();

            // Create a new delivery object and add it to the database
            var newDelivery = new Shipment
            {
                DriverEmail = selectedDriver.Email,
                DriverName = selectedDriver.FirstName + " " + selectedDriver.LastName,
                Address = gameOrder.DeliveryAddress,
                CustomerName = gameOrder.CustomerName,
                CustomerEmail = gameOrder.CustomerEmail,
                estShipDate = DateTime.Now.ToLongDateString(),
                OrderId = id,
            };


            db.Entry(mybill).State = EntityState.Modified;
            db.Entry(gameOrder).State = EntityState.Modified;
            db.Shipments.Add(newDelivery);
            db.SaveChanges();

            double TotalAmount = TotalPrice;


            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = UrlLink + "/Home/ThankYou/"+id;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details

            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            //onceOffRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = TotalAmount;
            onceOffRequest.item_name = "Payment made for an order";
            onceOffRequest.item_description = "Payment made for the order at our store";

            // BusinessLogic.UpdateRoomsAvailable(roomBooking.RoomId);
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            //cartService.EmptyCart();

            return Redirect(redirectUrl);
        }

        public ActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            adHocRequest.merchant_id = this.payFastSettings.MerchantId;
            adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
            adHocRequest.return_url = this.payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
            adHocRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            adHocRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            adHocRequest.m_payment_id = "";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Error()
        {
            return View();
        }

        #endregion Methods
    }
}