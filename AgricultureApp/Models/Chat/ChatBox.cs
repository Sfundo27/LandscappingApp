using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Chat
{
	public class Chatter
	{
		[Key]
		public int Id { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
	}

	public class ChatBox
	{
		[Key]
		public int Id { get; set; }
		public int chatterId { get; set; }
		public string date { get; set; }
		public string time { get; set; }
		public bool isRead { get; set; }
		public string message { get; set; }
		public string sender { get; set; }
		public string reciever { get; set; }
	}
}