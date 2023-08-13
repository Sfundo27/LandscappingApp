using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Product
{
    public class Delivery
    {
        public int Id { get; set; }
		public int orderId { get; set; }
		public string DriverName { get; set; }
        public string DriverEmail { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string DeliveryDate { get; set; }
        public bool isPickUp { get; set; }
        public bool isDelivered { get; set; }
    }
}