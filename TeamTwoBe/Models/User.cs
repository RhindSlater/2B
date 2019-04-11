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
        public string Password { get; set; } //Later needs to be hashed for security.
        public string City { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } //Joe: Either Homephone or Mobile. Must be string datatype to store all numbers properly.
        public AccountType UserLevel { get; set; }
        public List<User> Following { get; set; }
        public List<User> Follower { get; set; }
        public List<Card> Wishlist { get; set; }
        public List<Sale> ShoppingCart { get; set; }
    }
}