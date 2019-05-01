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



            return View();
        }

        public async Task<ActionResult> GetCardName(string GetCardName)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("https://db.ygoprodeck.com/api/")
            };

            HttpResponseMessage response = await client.GetAsync($"v4/cardinfo.php?fname={GetCardName}");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();

                return View("Wishlist",li);
            }



            return View();
        }


        public ActionResult addToWishList(string search)
        {


            return View(db.Cards.Where(x => x.name.StartsWith(search)).ToList());
        }



    }
}