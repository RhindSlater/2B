using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class ListCardListSale
    {
        public List<Card> Cards { get; set; }
        public List<Sale> Sales { get; set; }
        public List<User> Users { get; set; }
    }
}