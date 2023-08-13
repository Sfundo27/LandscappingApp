using AgricultureApp.Models.Product;
using AgricultureApp.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Shipping
{
	public class Shipment
	{
		public int shipmentID { get; set; }
		public int OrderId { get; set; }

		public string estShipDate { get; set; }

		public string DriverName { get; set; }
		public string DriverEmail { get; set; }

		public string Address { get; set; }
		public string CustomerName { get; set; }
		public string CustomerEmail { get; set; }
		public bool isPickUp { get; set; }
		public bool isDelivered { get; set; }
	}

	public class OrderViewModel
	{
		public Order Order { get; set; }
		public List<OrderItem> OrderItems { get; set; }
	}

}