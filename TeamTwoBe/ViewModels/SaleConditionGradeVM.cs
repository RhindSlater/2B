using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.Views.ViewModels
{
    public class SaleConditionGradeVM
    {
        public User MyUser { get; set; }

        public Sale MySale { get; set; }

        //I don't think this needs to be a list. ~Joe
        public List<Condition> MyCondition { get; set; }

        public Grade MyGrade { get; set; }

    }
}