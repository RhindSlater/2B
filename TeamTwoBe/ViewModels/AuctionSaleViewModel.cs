using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class AuctionSaleViewModel
    {
        public List<Sale> MySales { get; set; }
        public List<Bid> MyBids { get; set; }
    }
}