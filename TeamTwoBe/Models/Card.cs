using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Card
    {
        public int ID { get; set; }
        //We manually choose this from our list of card themes.
        public CardType Cardtype { get; set; }

        //Below is all the info we want to be pulled from the two API's. Can add more later as required.
        [DisplayName("Name")]
        public string name { get; set; }
        [DisplayName("Set")]
        public string print_tag { get; set; }
        public string apiID { get; set; }
        public double low { get; set; }
        public double average { get; set; }
        public double high { get; set; }
        public string rarity { get; set; }
        public string image_url { get; set; }
        public List<User> Wishers { get; set; }
        public List<User> CollectionOwners { get; set; }
    }
}