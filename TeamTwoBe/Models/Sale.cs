using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Sale
    {
        public int ID { get; set; }
        [Required]
        public User Seller { get; set; }
        [Required]
        public Card Card { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public Condition CardCondition { get; set; }
        [Required]
        public Grade CardGrade { get; set; }
        [Required]
        public bool ForAuction { get; set; }
        public User Buyer { get; set; }
        public bool IsSold { get; set; }
    }
}