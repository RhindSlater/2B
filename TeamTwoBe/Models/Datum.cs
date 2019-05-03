using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Datum
    {
        public string name { get; set; }
        public string print_tag { get; set; }
        public string rarity { get; set; }
        public PriceData price_data { get; set; }
    }
}