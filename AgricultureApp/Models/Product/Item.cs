using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Product
{
	public class Item
	{
		public int id { get; set; }
		public string name { get; set; }
		public int catID { get; set; }
		public string department { get; set; }
		public virtual Category Category { get; set; }
		public string description { get; set; }
		public int qty { get; set; }
		public double price { get; set; }
		public byte[] picture { get; set; }
		public double avRating { get; set; }
		public int numOfRate { get; set; }
	}

	public class Category
	{
		public int id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
	}
}