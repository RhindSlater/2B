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
        public User Reviewer { get; set; }

        public User Reviewee { get; set; }

        public Sale CardReviewed { get; set; }

        public string ReviewGiven { get; set; }

        [Range(0, 5)]
        public int StarRating { get; set; } //Out of 5.

        //If a trade takes longer than 1 month to review, you cannot give a review anymore.

    }
}