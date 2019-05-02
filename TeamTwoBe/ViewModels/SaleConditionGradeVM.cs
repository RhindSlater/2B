using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.Views.ViewModels
{
    public class SaleConditionGradeVM
    {
        //public User MyUser { get; set; }

        public Sale MySale { get; set; }

        //public Grade grade { get; set; }

        //public User user { get; set; }

        //[Display(Name = "Card name")]
        //public Card MyCard { get; set; }

        //[Display(Name = "Conditions")]
        //public string MyCondition { get; set; }

        //[Display(Name = "Grades")]
        //public string MyGrade { get; set; }

        public List<Card> MyCards { get; set; }

        public float Price { get; set; }
        public int ID { get; set; }
        public bool ForAuction { get; set; }




    }
}