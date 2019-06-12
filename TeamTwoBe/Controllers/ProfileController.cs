﻿using Newtonsoft.Json.Linq;
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

        public bool checkCookie() //check if same ipaddress
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
        public ActionResult Shoppingcart()
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
            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("ShoppingCart.Card.CardType").Include("ShoppingCart.Seller").Include("ShoppingCart.CardGrade").Include("ShoppingCart.Watcher").Include("ShoppingCart.CardCondition").Where(x => x.ID == id).FirstOrDefault();

            return View(user);
        }
        public ActionResult Following()
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
            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Follower").Where(x => x.ID == id).FirstOrDefault();
            List<User> lis = user.Follower;
            List<Sale> li = new List<Sale>();
            foreach (var i in lis)
            {
                var a = db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.ID == i.ID & x.IsSold == false & x.ForAuction == false).ToList();
                li.AddRange(a);
            }

            return View(li);

        }

        public ActionResult Watchlist()
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
            int id = Convert.ToInt32(Session["UserID"].ToString());

            User user = db.Users.Include("Watchlist.Card.CardType").Include("Watchlist.Seller").Include("Watchlist.CardGrade").Include("Watchlist.CardCondition").Where(x => x.ID == id).FirstOrDefault();

            return View(user);

        }

        //TODO: Show all the Reviews this user has given and been given, maybe in separate tables?
        public ActionResult MyReviews()
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
            //Current logged in User.
            int id = Convert.ToInt32(Session["UserID"].ToString());
            //Creating a list to be added to the view in a foreach with data below.
            List<UserReview> li = new List<UserReview>();

            //This gets the average of the reviewee's total ratings e.g. two received ratings of 1 and 3 would be avg of 2 etc.
            double userReviewsAvg = db.UserReviews.Where(x => x.Reviewee.ID == id).Select(u => u.StarRating).Average();
            //This counts all reviewers that a reviewer has given to the current logged in user.
            List<UserReview> userReviewsTotalReceived = db.UserReviews.Where(x => x.Reviewer.ID == id).ToList();

            //Check every Review in the DB to see if any ReviewerID matches the current user's ID to access the included values in the view.
            foreach (var i in db.UserReviews.Include("Reviewer").Include("Reviewee").Include("CardReviewed.Card").Where(x => x.Reviewer.ID == id))
            {
                li.Add(i);
            }
            //Check every Review in the DB to see if any RevieweeID matches the current user's ID to access the included values in the view.
            foreach (var i in db.UserReviews.Include("Reviewer").Include("Reviewee").Include("CardReviewed.Card").Where(x => x.Reviewee.ID == id))
            {
                li.Add(i);
            }
            //This view model is needed to count the user's reviews received total and average overall rating.
            UserReview_AverageAndTotalRatingsVM vm = new UserReview_AverageAndTotalRatingsVM()
            {
                userReview = li,
                averageRatings = userReviewsAvg,
                totalReviews = userReviewsTotalReceived
            };
            return View(vm);

        }

        public ActionResult removeWishlist(int id)
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

            Models.Card card = db.Cards.Where(x => x.ID == id).FirstOrDefault();

            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Wishlist").Where(x => x.ID == id).FirstOrDefault();
            user.Wishlist.Remove(card);
            db.SaveChanges();

            return RedirectToAction("Wishlist", new { id = id });
        }

        public ActionResult removeCollection(int id)
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

            Models.Card card = db.Cards.Where(x => x.ID == id).FirstOrDefault();

            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Collection").Where(x => x.ID == id).FirstOrDefault();
            user.Collection.Remove(card);
            db.SaveChanges();


            return RedirectToAction("Collection", new { id = id });
        }

        public ActionResult removeFromWatchlist(int id)
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
            checkCookie();
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
            checkCookie();
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
                Session["ShoppingCart"] = user.ShoppingCart.Count();
            }
            if (user.Watchlist.Contains(sale) == false)
            {
                user.Watchlist.Add(sale);
                db.SaveChanges();
            }
            return RedirectToAction("Shoppingcart");
        }

        //This method will use Stripe API to send an email to the buyer/winner of the sale.

        public ActionResult purchaseCard(int id)
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

            Sale sale = db.Sales.Include("Shopper").Include("Watcher").Include("Seller.ShoppingCart").Include("Seller.Watchlist").Include("Card").Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());

            User user = db.Users.Include("ShoppingCart").Where(x => x.ID == id).FirstOrDefault();

            sale.IsSold = true;
            sale.Buyer = user;

            foreach (var i in sale.Shopper)
            {
                i.ShoppingCart.Remove(sale);

            }
            foreach (var i in sale.Watcher)
            {
                i.Watchlist.Remove(sale);
            }
            Notification notify = new Notification()
            {
                Date = DateTime.Now,
                Title = "Card Sold",
                Message = $"{user.Username} has purchased your {sale.Card.name} for ${sale.Price}.",
                Seen = false,
                NotifyUser = sale.Seller,
            };
            db.Notifications.Add(notify);
            notify = new Notification()
            {
                Date = DateTime.Now,
                Title = "Card bought",
                Message = $"You have successfully purchased {sale.Card.name} from {sale.Seller.Username}. You will receive an email on how to pay for your card.",
                Seen = false,
                NotifyUser = user,
            };
            Session["ShoppingCart"] = sale.Seller.ShoppingCart.Count();
            db.Notifications.Add(notify);
            db.SaveChanges();
            return RedirectToAction("Won");
        }



        //This method initially charges the seller 10% the price of the card they are selling to be verified by us for the buyer.
        [HttpPost]
        public ActionResult purchaseVerifiedCard()
        {
            checkCookie();
            StripeConfiguration.SetApiKey("sk_test_P6m1FrtIXMp4Eb8vxViEhofQ00VVKk9gpa");

            //id is first set to mean the saleID.
            int id = Convert.ToInt32(Session["saleID"].ToString());

            //This is the current sale, where id is at this point, meaning the sale ID.
            Sale sale = db.Sales.Include("Shopper").Include("Watcher").Include("Seller.ShoppingCart").Include("Seller.Watchlist").Include("Card").Where(x => x.ID == id).FirstOrDefault();

            //At this point we've now changed id to be meaning the User ID.
            id = Convert.ToInt32(Session["UserID"].ToString());

            //This is the buyer with this buyer's shopping cart matching our id right above.
            User user = db.Users.Include("ShoppingCart").Where(x => x.ID == id).FirstOrDefault();

            Money moni = new Money()
            {
                MySale = sale,
                MyMoney = Convert.ToInt32(sale.Price * 100)
            };

            if (moni.MyMoney < 500)
            {
                moni.MyMoney = 500;
            }

            var options2 = new ChargeCreateOptions
            {
                Amount = moni.MyMoney / 10,
                Currency = "nzd",
                Description = $"Charge for {@sale.Seller.Username}",
                SourceId = "tok_visa", // obtained with Stripe.js, this is the payment method needed to be attached!

            };
            var service2 = new ChargeService();
            Charge charge = service2.Create(options2);

            if (options2 != null)
            {
                moni.MySale.IsSold = true;
                moni.MySale.Buyer = user;
                moni.MySale.IsVerified = true;

                foreach (var i in sale.Shopper)
                {
                    i.ShoppingCart.Remove(sale);
                }

                foreach (var i in sale.Watcher)
                {
                    i.Watchlist.Remove(sale);
                }
                Notification notify = new Notification()
                {
                    Date = DateTime.Now,
                    Title = "Card Sold",
                    Message = $"{user.Username} has purchased your {sale.Card.name} with verification for ${sale.Price}. Please send the card to 2B to be verified",
                    Seen = false,
                    NotifyUser = sale.Seller,
                };
                db.Notifications.Add(notify);

                notify = new Notification()
                {
                    Date = DateTime.Now,
                    Title = "Card bought",
                    Message = $"You have successfully purchased {sale.Card.name} with verification. You will be notified when your card has been verified by 2B and has been shipped.",
                    Seen = false,
                    NotifyUser = user,
                };
                db.Notifications.Add(notify);
                db.SaveChanges();
                Session["ShoppingCart"] = sale.Seller.ShoppingCart.Count();

                return RedirectToAction("Won");
            }
            return View(moni);

        }

        //This int id is the sale id. This one is for the view to enter info first...
        public ActionResult purchaseVerifiedCard(int id)
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

        //This is a list of all the items a user has bought. Checks database for every Sale == IsSold and BuyerID against the current logged in User.
        public ActionResult Won()
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

            int id = Convert.ToInt32(Session["UserID"].ToString());
            var li = db.Sales.Include("CardGrade").Include("Watcher").Include("CardCondition").Include("Card.Cardtype").Include("Seller").Include("Buyer").Where(x => x.IsSold == true & x.Buyer.ID == id).ToList();
            var lis = db.Bids.Include("Item").Where(x => x.Bidder.ID == id).ToList();
            AuctionSaleViewModel vm = new AuctionSaleViewModel()
            {
                MySales = li,
                MyBids = lis,
            };

            return View(vm);
        }
        public ActionResult Sold()
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

            int id = Convert.ToInt32(Session["UserID"].ToString());
            var li = db.Sales.Include("CardGrade").Include("Watcher").Include("CardCondition").Include("Card.Cardtype").Include("Seller").Include("Buyer").Where(x => x.IsSold == true & x.Seller.ID == id);

            return View(li.ToList());
        }


        public ActionResult addToWatchlist(int id)
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
            checkCookie();
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            User user = db.Users.Include("Wishlist.CardType").Where(x => x.ID == id).FirstOrDefault();
            if (user.ID.ToString() == Session["UserID"].ToString())
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
            checkCookie();
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
        public ActionResult checkReviewed(int id)
        {
            List<Sale> li = db.Sales.Include("Buyer").Where(x => x.Buyer.ID == id).ToList();
            if (li.Count < 1)
            {
                return Json("User has no cards purchased", JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<string> lis = new List<string>();
                foreach(var i in li)
                {
                    if(db.UserReviews.Include("CardReviewed").Include("Reviewer").Where(x => x.Reviewer.ID == i.Buyer.ID & x.CardReviewed.ID == i.ID).FirstOrDefault() == null)
                    {
                        lis.Add("true");
                        lis.Add(i.ID.ToString());
                    }
                    else
                    {
                        lis.Add("false");
                        lis.Add("0");
                    }
                }
                return Json(lis, JsonRequestBehavior.AllowGet);
            }
        }
    }
}