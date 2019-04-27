using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;

namespace TeamTwoBe.Controllers
{
    public class ProfileController : Controller
    {
        private Context db = new Context();

        private HttpClient yugiohApi;

        // GET: WishlistIndex/2007/kuri
        public ActionResult WishlistIndex(string search)
        {

            //Next, check if they don't have any cards in wishlist, they should be able to search for some.
            if(search == null)
            {
                return View();
            }
            else
            {
                return View(db.Cards.Where(x => x.name.StartsWith(search)).ToList());
            }

        }


    }
}