using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class User
    {
        public int ID { get; set; }

        [DisplayName("*First Name")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [DisplayName("*Last Name")]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [DisplayName("*Username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [DisplayName("*Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        
        public string City { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DisplayName("*Email")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string cookie { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }

        public string DisplayPicture { get; set; }
        public AccountType UserLevel { get; set; }

        [InverseProperty("Follower")]
        public List<User> Following { get; set; }


        [InverseProperty("Following")]
        public List<User> Follower { get; set; }


        [InverseProperty("Wishers")]
        public List<Card> Wishlist { get; set; }


        [InverseProperty("Shopper")]
        public List<Sale> ShoppingCart { get; set; }


        [InverseProperty("Watcher")]
        public List<Sale> Watchlist { get; set; }


        [InverseProperty("CollectionOwners")]
        public List<Card> Collection { get;  set; }
    }
}