using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class AdminViewModel
    {
        public List<User> Users { get; set; }
        public List<Sale> Sales { get; set; }
        public List<Sale> Verified { get; set; }
        public List<Bid> Bids { get; set; }
        public Notification notify { get; set; }
    }
}