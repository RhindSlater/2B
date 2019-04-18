using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamTwoBe.Models;

namespace TeamTwoBe.Views.ViewModels
{
    public class SaleConditionGradeVM
    {
        public Sale MySale { get; set; }
        public Condition MyCondition { get; set; }
        public Grade MyGrade { get; set; }

    }
}