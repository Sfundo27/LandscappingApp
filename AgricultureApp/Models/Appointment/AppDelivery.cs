using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Product
{
    public class AppDelivery
    {
        public int Id { get; set; }
		public int orderId { get; set; }
		public string FumName { get; set; }
        public string FumEmail { get; set; }
        public string Name { get; set; }
        public string pass { get; set; }
        public string Address { get; set; }
        public string DeliveryDate { get; set; }
        public bool isPickUp { get; set; }
        public bool isConfirmed { get; set; }
        public bool isDelivered { get; set; }
    }
}