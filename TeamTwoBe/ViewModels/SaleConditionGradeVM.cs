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
        public Sale MySale { get; set; }
        public User MyUser { get; set; }

        [Display(Name = "Card name")]
        public Card MyCard { get; set; }

        [Display(Name = "Conditions")]
        public string MyCondition { get; set; }

        [Display(Name = "Grades")]
        public string MyGrade { get; set; }

    }
}