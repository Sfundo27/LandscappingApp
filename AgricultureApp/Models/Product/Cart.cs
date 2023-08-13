using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Product
{
    public class CartItem
    {
        public int Id { get; set; }
        public int itemID { get; set; }
        public int Quantity { get; set; }
        public string Product { get; set; }
        public byte[] Picture { get; set; }
		public double price { get; set; }
		public string userID { get; set; }
		public virtual Cart Cart { get; set; }
		public virtual Item Item { get; set; }
    }

    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}