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
        public Card Card { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public float Price { get; set; }

        [Required]
        [Display(Name = "Card Condition")]
        public Condition CardCondition { get; set; }

        [Required]
        [Display(Name = "Card grade")]
        public Grade CardGrade { get; set; }

        [Display(Name = "Auction")]
        public bool ForAuction { get; set; }
        public User Buyer { get; set; }
        [Required]
        public User Seller { get; set; }
        //Starts as false. If true, uses the purchaseVerifiedCard function from Stripe API.
        public bool IsVerified { get; set; }
        //Starts as fasle. Changes to true if purchaseCard or purchaseVerifiedCard has been clicked.
        public bool IsSold { get; set; }
        public DateTime UploadDate { get; set; }
        public List<User> Shopper { get; set; }
        public List<User> Watcher { get; set; }
    }
}