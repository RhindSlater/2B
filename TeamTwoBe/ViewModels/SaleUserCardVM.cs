using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class SaleUserCardVM
    {
        public User MyUser{ get; set; }
        public Sale MySale { get; set; }
        public Card MyCard { get; set; }
    }
}