using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Card
    {
        public int ID { get; set; }
        public CardType Cardtype { get; set; }
        public string Name { get; set; }
        public float PriceLow { get; set; }
        public float PriceAverage { get; set; }
        public float PriceHigh { get; set; }
        public List<User> Wishlist { get; set; }
    }
}