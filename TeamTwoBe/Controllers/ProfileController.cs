using Newtonsoft.Json.Linq;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        HttpClient yugiohPriceApi = new HttpClient()
        {
            BaseAddress = new Uri("http://yugiohprices.com/api/")
        };

        public ActionResult Shoppingcart()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("ShoppingCart.Card.CardType").Include("ShoppingCart.Seller").Include("ShoppingCart.CardGrade").Include("ShoppingCart.Watcher").Include("ShoppingCart.CardCondition").Where(x => x.ID == id).FirstOrDefault();

            return View(user);

        }

        public ActionResult Watchlist()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            int id = Convert.ToInt32(Session["UserID"].ToString());

            User user = db.Users.Include("Watchlist.Card.CardType").Include("Watchlist.Seller").Include("Watchlist.CardGrade").Include("Watchlist.CardCondition").Where(x => x.ID == id).FirstOrDefault();

            return View(user);

        }

        public ActionResult removeWishlist(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Models.Card card = db.Cards.Where(x => x.ID == id).FirstOrDefault();

            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Wishlist").Where(x => x.ID == id).FirstOrDefault();
            user.Wishlist.Remove(card);
            db.SaveChanges();

            return RedirectToAction("Wishlist", new { id = id });
        }

        public ActionResult removeCollection(int id)
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
            User user = db.Users.Include("Collection").Where(x => x.ID == id).FirstOrDefault();
            user.Collection.Remove(card);
            db.SaveChanges();


            return RedirectToAction("Collection", new { id = id });
        }

        public ActionResult removeFromWatchlist(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            Sale sale = db.Sales.Include("Seller").Where(x => x.ID == id).FirstOrDefault();
            if (sale.Seller.ID.ToString() == Session["UserID"].ToString())
            {
                return RedirectToAction("Index");
            }
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Watchlist").Where(x => x.ID == id).FirstOrDefault();

            user.Watchlist.Remove(sale);
            db.SaveChanges();

            return RedirectToAction("Watchlist");
        }

        public ActionResult removeFromShoppingCart(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            Sale sale = db.Sales.Include("Seller").Where(x => x.ID == id).FirstOrDefault();
            if (sale.Seller.ID.ToString() == Session["UserID"].ToString())
            {
                return RedirectToAction("Index");
            }
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("ShoppingCart").Where(x => x.ID == id).FirstOrDefault();

            user.ShoppingCart.Remove(sale);
            db.SaveChanges();
            Session["ShoppingCart"] = user.ShoppingCart.Count();

            return RedirectToAction("ShoppingCart");
        }

        public ActionResult addShoppingcart(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Sale sale = db.Sales.Include("Seller").Where(x => x.ID == id).FirstOrDefault();

            //If you are the seller of this card, then you get redirected to the index view.
            if (sale.Seller.ID.ToString() == Session["UserID"].ToString())
            {
                return RedirectToAction("Index");
            }
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("ShoppingCart").Include("Watchlist").Where(x => x.ID == id).FirstOrDefault();

            if (user.ShoppingCart.Contains(sale) == false)
            {
                user.ShoppingCart.Add(sale);
                db.SaveChanges();
            }
            if (user.Watchlist.Contains(sale) == false)
            {
                user.Watchlist.Add(sale);
                db.SaveChanges();
                Session["ShoppingCart"] = user.ShoppingCart.Count();
            }
            return RedirectToAction("Shoppingcart");
        }

        //This method will use Stripe API to send an email to the buyer/winner of the sale.

        public ActionResult purchaseCard(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Sale sale = db.Sales.Include("Seller").Where(x => x.ID == id).FirstOrDefault();

            if (sale.IsVerified == true)
            {
                return RedirectToAction("purchaseVerifiedCard");
            }

            return View();
        }

        //This method initially charges the seller 10% the price of the card they are selling to be verified by us for the buyer.
        [HttpPost]
        public ActionResult purchaseVerifiedCard()
        {
            StripeConfiguration.SetApiKey("sk_test_P6m1FrtIXMp4Eb8vxViEhofQ00VVKk9gpa");

            //Doing this code below does not give us access to the Sale's Seller!!!
            //Sale sale = db.Sales.Find(Session["saleID"]);

            //Sale sale = db.Sales.Include("Seller").Where(x => x.ID == id).FirstOrDefault();

            int id = Convert.ToInt32(Session["saleID"].ToString());

            Sale sale = db.Sales.Include("Shopper").Include("Watcher").Include("Seller.ShoppingCart").Include("Seller.Watchlist").Include("Card").Where(x => x.ID == id).FirstOrDefault();
            //Session["saleID"] = null;

            Money moni = new Money()
            {
                MySale = sale,
                MyMoney = Convert.ToInt32(sale.Price * 100)
            };

            var options2 = new ChargeCreateOptions
            {
                Amount = moni.MyMoney / 10,
                Currency = "nzd",
                Description = $"Charge for {@sale.Seller.Username}",
                SourceId = "tok_visa", // obtained with Stripe.js, this is the payment method needed to be attached!
            };
            var service2 = new ChargeService();
            Charge charge = service2.Create(options2);

            if(options2 != null)
            {
                moni.MySale.IsSold = true;

                foreach(var i in sale.Shopper)
                {
                    i.ShoppingCart.Remove(sale);
                    
                }

                foreach (var i in sale.Watcher)
                {
                    i.Watchlist.Remove(sale);

                }
                ViewData["success"] = $"Successfully purchased {sale.Card.name} for {options2.Amount}!";
                db.SaveChanges();
                return RedirectToAction("Shoppingcart");
            }
            else
            {
                return View(moni);
            }
        }

        //This int id is the sale id. This one is for the view to enter info first...
        public ActionResult purchaseVerifiedCard(int id)
        {
            //var viewModel = ReturnCard();

            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Sale sale = db.Sales.Include("Card").Include("Seller").Where(x => x.ID == id).FirstOrDefault();

            Session["saleID"] = sale.ID;

            //If you are the seller of this card, then you get redirected to the index view.
            if (sale.Seller.ID.ToString() == Session["UserID"].ToString())
            {
                return RedirectToAction("Index");
            }

            id = Convert.ToInt32(Session["UserID"].ToString());

            Money moni = new Money()
            {
                MySale = sale,
                MyMoney = Convert.ToInt32(sale.Price * 100)
            };

            sale.IsVerified = true;

            return View(moni);
        }

        public ActionResult addToWatchlist(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Sale sale = db.Sales.Include("Seller").Where(x => x.ID == id).FirstOrDefault();
            if (sale.Seller.ID.ToString() == Session["UserID"].ToString())
            {
                return RedirectToAction("Index");
            }
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Watchlist").Where(x => x.ID == id).FirstOrDefault();

            if (user.Watchlist.Contains(sale) == false)
            {
                user.Watchlist.Add(sale);
                db.SaveChanges();
            }
            return RedirectToAction("Watchlist");
        }

        public ActionResult Wishlist(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            User user = db.Users.Include("Wishlist.CardType").Where(x => x.ID == id).FirstOrDefault();
            if(user.ID.ToString() == Session["UserID"].ToString())
            {
                if (user.Wishlist.Count == 0)
                {
                    return RedirectToAction("Wishlist", "Card", user.ID);
                }
            }
            return View(user);
        }

        public ActionResult Collection(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            User user = db.Users.Include("Collection.CardType").Where(x => x.ID == id).FirstOrDefault();
            if (user.ID.ToString() == Session["UserID"].ToString())
            {
                if (user.Collection.Count == 0)
                {
                    return RedirectToAction("Collection", "Card", user.ID);
                }
            }
            return View(user);
        }
    }
}