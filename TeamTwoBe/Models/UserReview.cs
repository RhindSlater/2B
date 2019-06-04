using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class UserReview
    {
        public int ID { get; set; }
        //Buyer
        public User Reviewer { get; set; }
        //Seller of sale.
        public User Reviewee { get; set; }
        [Required]
        public Sale CardReviewed { get; set; } //This should be required!
        public string ReviewGiven { get; set; }

        [Range(0, 5)]
        public int StarRating { get; set; } //Out of 5.

        //If a trade takes longer than 1 month to review, you should not be able to give a review anymore.

    }
}