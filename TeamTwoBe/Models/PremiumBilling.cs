using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class PremiumBilling
    {
        public int ID { get; set; }
        public User Member { get; set; }
        public DateTime Date { get; set; }
        public DateTime NextBillingDate { get; set; }
        public double Amount { get; set; }
    }
}