using AgricultureApp.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Product
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
		public string OrderNo { get; set; }
		public string userId { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        public string AssistanceEmail { get; set; }
        public string AssistanceName { get; set; }

        [DataType(DataType.Time)]
        public string ApproveTime { get; set; }
        [DataType(DataType.Date)]
        public string ApproveDate { get; set; }

        public string expDelDate { get; set; }
        public string addressValid { get; set; }
        public string BookRef { get; set; }

        [Display(Name = "Picture")]
        //[DataType(DataType.Upload)]
        public byte[] Picture { get; set; }
        public double Total { get; set; }
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; }

        [DataType(DataType.Time)]
        public string ShipTime { get; set; }
        public string StartShipTime { get; set; }
        [DataType(DataType.Date)]
        public string ShipDate { get; set; }
        public string StartShipDate { get; set; }
        public string receiverName { get; set; }

        public int DriverID { get; set; }
        public string dName { get; set; }
        public string dEmail { get; set; }
        public string dVehicle { get; set; }
        public string carPlate { get; set; }
        public string shipmentStatus { get; set; }
        public string shipmentByEmail { get; set; }
        public string shipmentByName { get; set; }
        public int shipmentExpDay { get; set; }

        public string ArrivalTime { get; set; }
        public string ArrivalDate { get; set; }

        public string receiveDate { get; set; }
        public string receiveTime { get; set; }
        public string receiverEmail { get; set; }

        public string EarlyLateParcel { get; set; }
        public string PaymentStatus { get; set; }

		public bool isUpdated { get; set; }
		public bool isPaid { get; set; }
		public bool isBilled { get; set; }
		public bool isPickUp { get; set; }
		public bool isDelivered { get; set; }
		public byte[] signature { get; set; }




	}

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public byte[] Picture { get; set; }

        public string itemName { get; set; }
        public int Quantity { get; set; }

        public double UnitPrice { get; set; }
        public double total { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual Item Item { get; set; }

        public List<Order> orders { get; set; }
        public List<OrderItem> orderItems { get; set; }
        public List<OrderHistory> histories { get; set; }
        public List<Delivery> deliveries { get; set; }
    }


    public class OrderHistory
	{
        [Key]
        public int Id { get; set; }
        public string userId { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }
        public string DeliveryDate { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public double TotalAmount { get; set; }

		public byte[] signature { get; set; }
	}

}