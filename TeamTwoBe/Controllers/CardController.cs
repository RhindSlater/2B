using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.Views.ViewModels;

namespace TeamTwoBe.Controllers
{
    public class CardController : Controller
    {
        private Context db = new Context();

        HttpClient yugiohApi = new HttpClient()
        {
            BaseAddress = new Uri("https://db.ygoprodeck.com/api/")
        };

        HttpClient yugiohPriceApi = new HttpClient()
        {
            BaseAddress = new Uri("http://yugiohprices.com/api/")
        };

        [HttpPost]
        public async Task<ActionResult> apiPrice(string dropboxvalue)
        {
            HttpResponseMessage resp = await yugiohPriceApi.GetAsync($"get_card_prices/{dropboxvalue}");
            if (resp.IsSuccessStatusCode)
            {
                var rsp = await resp.Content.ReadAsStringAsync();
                var li = JObject.Parse(rsp).ToObject<RootObject>();

                SaleConditionGradeVM salevm = new SaleConditionGradeVM()
                {
                    MyCard = dropboxvalue,
                    MyDatum = new List<Datum>(),
                    MyCards = new List<Card>(),
                };


                foreach (var i in li.data)
                {
                    if (i.price_data.status == "success")
                    {
                        Card card = new Card()
                        {
                            apiID = $"{dropboxvalue} {i.print_tag} {i.rarity}",
                            name = dropboxvalue,
                            rarity = i.rarity,
                            print_tag = i.print_tag,
                            Cardtype = db.CardTypes.Find(1),
                            image_url = "http://www.ygo-api.com/api/Images/cards/" + dropboxvalue,
                            average = i.price_data.data.prices.average,
                            high = i.price_data.data.prices.high,
                            low = i.price_data.data.prices.low,
                        };
                        salevm.MyDatum.Add(i);
                        salevm.MyCards.Add(card);
                        if (db.Cards.Where(x => x.apiID == card.apiID).FirstOrDefault() == null)
                        {
                            db.Cards.Add(card);
                        }
                    }
                }
                db.SaveChanges();


                return View("Wishlist", salevm);
            }
            SaleConditionGradeVM salev = new SaleConditionGradeVM();
            return View("Wishlist", salev);
        }

        public async Task<ActionResult> Wishlist()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            HttpResponseMessage response = await yugiohApi.GetAsync($"v4/cardinfo.php?");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();

                SaleConditionGradeVM uservm = new SaleConditionGradeVM()
                {
                    MyCards = li,
                };
                return View(uservm);
            }
            SaleConditionGradeVM vm = new SaleConditionGradeVM();
            vm.MyCards = new List<Card>();
            return View(vm);
        }

        //This searches for a card containing any part of the word that is being searched for e.g. dark
        public ActionResult addCardToWishlist(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            Card card = db.Cards.Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Wishlist").Where(x => x.ID == id).FirstOrDefault();
            if (user.Wishlist.Contains(card) == false)
            {
                user.Wishlist.Add(card);
                db.SaveChanges();
            }

            return RedirectToAction("Wishlist", "Profile", new { id });
        }
    }
}