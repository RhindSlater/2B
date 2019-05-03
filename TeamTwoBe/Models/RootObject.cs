using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class RootObject
    {
        public string status { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Data
    {
        public List<object> listings { get; set; }
        public Prices prices { get; set; }
    }

    public class PriceData
    {
        public string status { get; set; }
        public Data data { get; set; }
    }
}