using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public float Phone { get; set; }
        public AccountType UserLevel { get; set; }
        public List<User> Following { get; set; }
        public List<User> Follower { get; set; }
        public List<Card> Wishlist { get; set; }
        public List<Sale> ShoppingCart { get; set; }
    }
}