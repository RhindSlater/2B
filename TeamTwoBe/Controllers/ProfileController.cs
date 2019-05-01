using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.ViewModels;

namespace TeamTwoBe.Controllers
{
    public class ProfileController : Controller
    {
        private Context db = new Context();

        HttpClient yugiohApi = new HttpClient()
        {
            BaseAddress = new Uri("https://db.ygoprodeck.com/api/")
        };

        // GET: WishlistIndex/2007/kuri
        public ActionResult Wishlist(User user)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }

            int id = Convert.ToInt32(Session["UserID"]);
            user = db.Users.Where(x => x.ID == id).FirstOrDefault();



            return View();
        }

        //This searches for a card containing any part of the word that is being searched for e.g. dark
        public async Task<ActionResult> GetCardName(string GetCardName)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }

            //I think this grabs the text being submitted? ~Joe
            HttpResponseMessage response = await yugiohApi.GetAsync($"v4/cardinfo.php?fname={GetCardName}");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();

                return View("Wishlist",li);
            }

            return View();
        }

        //This adds a card to your wishlist when you click the add to wishlist button of any searched card.
        public async Task<ActionResult> addToWishList(string id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }

            Card card = new Card();

            HttpResponseMessage response = await yugiohApi.GetAsync($"v4/cardinfo.php?name={id}");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();

                card.name = id;

                db.Cards.Add(card);
                db.SaveChanges();

                return View("Wishlist", li);
            }

            return View();
        }



    }
}