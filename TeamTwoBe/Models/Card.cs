using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Card
    {
        public int ID { get; set; }
        //We manually choose this from our list of card themes.
        public CardType Cardtype { get; set; }

        //Below is all the info we want to be pulled from the API. Can add more later as required.
        [DisplayName("Name")]
        public string name { get; set; }

        //Info we want but the API doesn't currently provide.
        public float PriceLow { get; set; }
        public float PriceAverage { get; set; }
        public float PriceHigh { get; set; }
        public string Rarity { get; set; }
        public string image_url { get; set; }
        public List<User> Wishers { get; set; }
    }
}