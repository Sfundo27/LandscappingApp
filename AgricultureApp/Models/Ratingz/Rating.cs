using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AgricultureApp.Models.Ratingz
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int RatingId { get; set; }
        public string CustomerID { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }

        [Range(0, 5)]
        public int Rank { get; set; }

        public string Comment { get; set; }
    }
}
