using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AgricultureApp.Models.Users;
using AgricultureApp.Models.MyBill;
using AgricultureApp.Models.Shipping;
using AgricultureApp.Models.Ratingz;

namespace AgricultureApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNo { get; set; }
        public bool ConfirmedEmail { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Assistant> Assistants { get; set; }

		public System.Data.Entity.DbSet<AgricultureApp.Models.Product.AppDelivery> AppDeliveries { get; set; }


		public System.Data.Entity.DbSet<AgricultureApp.Models.Appointment.Booking> Bookings { get; set; }

		public System.Data.Entity.DbSet<AgricultureApp.Models.Appointment.Fumigation> Fumigations { get; set; }

		public System.Data.Entity.DbSet<AgricultureApp.Models.Appointment.FumService> FumServices { get; set; }

		public System.Data.Entity.DbSet<AgricultureApp.Models.Appointment.FumTreatment> FumTreatments { get; set; }


        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.Item> Items { get; set; }

        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.Category> Categories { get; set; }
        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.Cart> Carts { get; set; }
        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.CartItem> CartItems { get; set; }
        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.Order> Orders { get; set; }
        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.OrderItem> OrderItems { get; set; }
        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.Delivery> Deliveries { get; set; }
        public System.Data.Entity.DbSet<AgricultureApp.Models.Product.OrderHistory> OrderHistories { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<OrderBill> OrderBills { get; set; }
        public DbSet<BillHistory> BillHistory { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Rating> Ratings { get; set; }

		public System.Data.Entity.DbSet<AgricultureApp.Models.Chat.ChatBox> ChatBoxes { get; set; }
		public System.Data.Entity.DbSet<AgricultureApp.Models.Chat.Chatter> Chatters { get; set; }
	}
}