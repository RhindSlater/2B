using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class HomeViewModel
    {
        public List<Sale> Followers { get; set; }
        public List<Sale> Recommended { get; set; }
        public List<Sale> Trending { get; set; }
        public List<Sale> UpcomingAuction { get; set; }
        public Sale CurrentAuction { get; set; }
    }
}