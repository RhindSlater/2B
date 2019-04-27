using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.ViewModels;

namespace TeamTwoBe.Controllers
{
    public class ProfileController : Controller
    {
        private Context db = new Context();

        private HttpClient yugiohApi;

        // GET: WishlistIndex/2007/kuri
        public ActionResult Wishlist(User user)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }

            int id = Convert.ToInt32(Session["UserID"]);
            user = db.Users.Where(x => x.ID == id).FirstOrDefault();

            return View(user);
        }


        public ActionResult addToWishList(string search)
        {


            return View(db.Cards.Where(x => x.name.StartsWith(search)).ToList());
        }



    }
}