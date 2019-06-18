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
        //used to query our database
        private Context db = new Context();

        //api used to search for every card
        HttpClient yugiohApi = new HttpClient()
        {
            BaseAddress = new Uri("https://db.ygoprodeck.com/api/")
        };

        //api used to obtain price information based on cards
        HttpClient yugiohPriceApi = new HttpClient()
        {
            BaseAddress = new Uri("http://yugiohprices.com/api/")
        };

        public bool checkCookie()
        {
            string userid = string.Empty;
            if (Request != null)
            {
                if (Request.Cookies["userid"] != null)
                {
                    var address = Request.UserHostAddress;
                    userid = Request.Cookies["userid"].Value;
                    User user = db.Users.Include("UserLevel").Include("ShoppingCart").Where(x => x.cookie == userid).FirstOrDefault();
                    if (user != null)
                    {
                        Session["UserID"] = user.ID;
                        Session["Username"] = user.Username;
                        Session["UserPic"] = user.DisplayPicture;
                        Session["ShoppingCart"] = user.ShoppingCart.Count();
                        Session["AccountLevel"] = user.UserLevel.ID.ToString();
                    }
                    return true;
                }
            }
            return false;
        }

        private void checkUserID()
        {
            if (Session["UserID"] == null)
            {
                RedirectToAction("login", "users");
                return;
            }
            if (Session["UserID"].ToString() == "0")
            {
                RedirectToAction("login", "users");
                return;
            }
        }

        [HttpPost]
        public async Task<ActionResult> apiPrice(string dropboxvalue)
        {
            //checks if cookies are saved
            checkCookie();

            //calls function from api
            HttpResponseMessage resp = await yugiohPriceApi.GetAsync($"get_card_prices/{dropboxvalue}");
            if (resp.IsSuccessStatusCode)
            {
                var rsp = await resp.Content.ReadAsStringAsync();
                //returns a list of data from our api
                var li = JObject.Parse(rsp).ToObject<RootObject>();

                //viewmodel needed to display all our cards, their pricing data and it's name
                SaleConditionGradeVM salevm = new SaleConditionGradeVM()
                {
                    MyCard = dropboxvalue,
                    MyDatum = new List<Datum>(),
                    MyCards = new List<Card>(),
                };

                //iterates through list of data and creates a new card adding it to the database if it returns success
                foreach (var i in li.data)
                {
                    //success means it successfully finds pricing for the card in your list
                    if (i.price_data.status == "success")
                    {
                        //creates a new card with all the new properties from the api
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

                        //adds data to a list of data
                        salevm.MyDatum.Add(i);
                        //adds card to a list of cards
                        salevm.MyCards.Add(card);

                        //makes sure the card is not in the database before adding it the database
                        if (db.Cards.Where(x => x.apiID == card.apiID).FirstOrDefault() == null)
                        {
                            db.Cards.Add(card);
                        }
                    }
                }
                //saves the changes
                db.SaveChanges();


                return View("Wishlist", salevm);
            }
            SaleConditionGradeVM salev = new SaleConditionGradeVM();
            return View("Wishlist", salev);
        }

        [HttpPost]
        public async Task<ActionResult> apiPrice2(string dropboxvalue)
        {
            checkCookie();
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
                        //makes sure not to add the card to the database if it already exists
                        if (db.Cards.Where(x => x.apiID == card.apiID).FirstOrDefault() == null)
                        {
                            salevm.MyCards.Add(card);
                            db.Cards.Add(card);
                        }
                        else
                        {
                            //if the card exists in the database then use the card from the database instead of adding a new card
                            Card colcard = db.Cards.Where(x => x.apiID == card.apiID).FirstOrDefault();
                            salevm.MyCards.Add(colcard);
                        }
                    }
                }
                db.SaveChanges();


                return View("Collection", salevm);
            }
            SaleConditionGradeVM salev = new SaleConditionGradeVM();
            return View("Collection", salev);
        }

        public async Task<ActionResult> Wishlist()
        {
            checkCookie();
            checkUserID();

            HttpResponseMessage response = await yugiohApi.GetAsync($"v4/cardinfo.php?");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                //converts a jarray to list of card
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


        public async Task<ActionResult> Collection()
        {
            checkCookie();
            checkUserID();

            //This searches for a card containing any part of the word that is being searched for e.g. dark
            HttpResponseMessage response = await yugiohApi.GetAsync($"v4/cardinfo.php?");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();
                //converts json to list of cards
                SaleConditionGradeVM uservm = new SaleConditionGradeVM()
                {
                    MyCards = li,
                };
                //returns list of cards to the view
                return View(uservm);
            }
            SaleConditionGradeVM vm = new SaleConditionGradeVM();
            vm.MyCards = new List<Card>();
            return View(vm);
        }

        //adds a card to your wishlist
        public ActionResult addCardToWishlist(int id)
        {
            checkCookie();
            checkUserID();
            Card card = db.Cards.Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Wishlist").Where(x => x.ID == id).FirstOrDefault();
            if (user.Wishlist.Contains(card) == false)
            {
                user.Wishlist.Add(card);
                db.SaveChanges();
            }

            return RedirectToAction("Wishlist", "Profile", new { id = id });
        }

        //adds a card to your collection
        public ActionResult addCardToCollection(int id)
        {
            checkCookie();
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
            User user = db.Users.Include("Collection").Where(x => x.ID == id).FirstOrDefault();
            if (user.Collection.Contains(card) == false)
            {
                user.Collection.Add(card);
                db.SaveChanges();
            }

            return RedirectToAction("Collection", "Profile", new { id = id });
        }
    }
}