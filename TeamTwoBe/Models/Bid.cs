using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Bid
    {
        public int ID { get; set; }
        public DateTime TimeStamps { get; set; } //Earliest and highest bid gets priority.
        public float BidAmount { get; set; }
        public Sale Item { get; set; }
        public User Bidder { get; set; }
    }
}