using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.MyBill
{
	public class OrderBill
	{

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string billID { get; set; }
        public int itemCode { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Surname")]
        public string Surname { get; set; }

        [DisplayName("Reg. Number")]
        public string RegNo { get; set; }
        
        [DisplayName("Reg. Number")]
        public string BillCode { get; set; }

        [Display(Name = "Order Amount")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public double OrderAmount { get; set; } = 0.00;

        [Display(Name = "Total Amount")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public double TotalAmount { get; set; } = 0.00;

        [Display(Name = "Status")]
        public string Status { get; set; }

        ApplicationDbContext db = new ApplicationDbContext();
    }
}