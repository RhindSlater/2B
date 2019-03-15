using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class AccountType
    {
        public int ID { get; set; }
        public string AccountLevel { get; set; } //lvl1 = freeuser, lvl2 = verified, lvl3 = admin
    }
}