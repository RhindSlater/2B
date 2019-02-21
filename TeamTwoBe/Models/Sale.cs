using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Sale
    {
        public int ID { get; set; }
        public User Seller { get; set; }
        public Card Card { get; set; }
        public float Price { get; set; }
        public Condition CardCondition { get; set; }
        public Grade CardGrade { get; set; }
        public bool ForAuction { get; set; }
    }
}