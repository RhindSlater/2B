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

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        //[StringLength(21,ErrorMessage = "Password is two short",MinimumLength = 6)]
        public string Password { get; set; } //Later needs to be hashed for security.

        public string City { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public string Phone { get; set; } //Joe: Either Homephone or Mobile. Must be string datatype to store all numbers properly.

        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }

        public string DisplayPicture { get; set; } //picture



        public AccountType UserLevel { get; set; }
        [InverseProperty("Follower")]
        public List<User> Following { get; set; }
        [InverseProperty("Following")]
        public List<User> Follower { get; set; }
        public List<Card> Wishlist { get; set; }
        public List<Sale> ShoppingCart { get; set; }
        [InverseProperty("Buyer")]
        public List<Sale> selling { get; set; }
    }
}