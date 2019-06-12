using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class UserReview_AverageAndTotalRatingsVM
    {
        public List<UserReview> ReceivedReview { get; set; }
        public List<UserReview> GivenReview { get; set; }
        public string userName { get; set; }
        public double averageRatings { get; set; }

        public List<UserReview> totalReviews { get; set; }


    }
}