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
        public List<Datum> MyDatum { get; set; }
        public List<Card> MyCards { get; set; }

        public float Price { get; set; }
        public int ID { get; set; }
        public string MyCard { get; set; }
        public string MyCard1 { get; set; }
        public bool ForAuction { get; set; }
        public bool fix { get; set; }

    }
}