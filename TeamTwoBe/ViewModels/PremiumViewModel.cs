using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.ViewModels
{
    public class PremiumViewModel
    {
        public User MyUser { get; set; }
        public PremiumBilling MyBilling { get; set; }
    }
}