using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Notification
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool Seen { get; set; }
        public DateTime Date { get; set; }
        public User NotifyUser { get; set; }
    }
}