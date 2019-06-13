using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class ProfileViewModel
    {
        public User MyUser { get; set; }
        public User LoggedInUser { get; set; }
        public List<Sale> MySales { get; set; }
        public List<Card> MyCollection { get; set; }
        public List<Sale> MyWatchList { get; set; }
        public List<Card> MyWishList { get; set; }
    }
}