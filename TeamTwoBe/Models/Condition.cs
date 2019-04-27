using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Condition
    {
        public int ID { get; set; }
        //Poor, Played, Light Played, Good, Excellent, Near Mint, Mint
        [DisplayName("Card Condition")]
        public string CardCondition { get; set; }  
    }
}