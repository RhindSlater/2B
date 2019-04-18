using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Condition
    {
        public int ID { get; set; }
        //Poor, Played, Light Played, Good, Excellent, Near Mint, Mint
        public string CardCondition { get; set; }  
    }
}