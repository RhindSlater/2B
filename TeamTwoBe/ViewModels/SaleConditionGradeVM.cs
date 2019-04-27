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
        public User MyUser { get; set; }

        public Sale MySale { get; set; }
<<<<<<< HEAD

        //I don't think this needs to be a list. ~Joe
        public List<Condition> MyCondition { get; set; }

        public Grade MyGrade { get; set; }
=======
        public User MyUser { get; set; }

        [Display(Name = "Card name")]
        public Card MyCard { get; set; }

        [Display(Name = "Conditions")]
        public string MyCondition { get; set; }

        [Display(Name = "Grades")]
        public string MyGrade { get; set; }
>>>>>>> JoesWork

    }
}