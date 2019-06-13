using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.ViewModels;

namespace TeamTwoBe.Controllers
{
    public class AuctionController : Controller
    {
        Context db = new Context();

        //Checks if user has cookies saved and refreshes their session id;
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

        //Places a bid on a card
        [HttpPost]
        public ActionResult placebid(float id)
        {
            //checks if you saved your cookies
            checkCookie();
            //checks if you are logged in
            if (Session["UserID"] != null)
            {
                //sets your user id
                int id1 = Convert.ToInt32(Session["UserID"].ToString());
                //finds user in the database with your user id
                User user = db.Users.Where(x => x.ID == id1).FirstOrDefault();
                //finds the current auction
                Sale sale = db.Sales.Where(x => x.IsSold == false & x.ForAuction == true).FirstOrDefault();


                //if no user found you are not logged in
                if (user != null)
                {
                    //Checks if you havealready placed a bid on current auction
                    if (db.Bids.Where(x => x.Item.ID == sale.ID & x.Bidder.ID == user.ID).FirstOrDefault() == null)
                    {
                        //create a new bid if you haven't
                        Bid bid = new Bid()
                        {
                            TimeStamps = DateTime.Now,
                            BidAmount = id,
                            Bidder = user,
                            Item = db.Sales.Where(x => x.IsSold == false & x.ForAuction == true).FirstOrDefault(),
                        };
                        //add bid to database
                        db.Bids.Add(bid);
                        //save changes
                        db.SaveChanges();
                        //notify you that your bid has been placed
                        return Json("Your bid has been placed", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //else you need to login
                    return Json("Please login", JsonRequestBehavior.AllowGet);
                }
            }
            //else you need to login
            return Json("Please login", JsonRequestBehavior.AllowGet);
        }


        //Action that ends an auction
        [HttpPost]
        public ActionResult AuctionEnd()
        {
            //finds the sale cuurently on auction
            Sale sale = db.Sales.Include("Card").Include("Shopper").Include("Watcher").Include("Seller.Collection").Where(x => x.ForAuction == true & x.IsSold == false).FirstOrDefault();

            //makes a list of all bids placed on the current auction
            List<Bid> bid = db.Bids.Include("Bidder").Where(x => x.Item.ID == sale.ID).OrderByDescending(x => x.BidAmount).ToList();

            //checks if the list is empty
            if (bid == null)
            {
                //tells seller that no one bid on the auction
                Notification notifyUnsold = new Notification()
                {
                    Date = DateTime.Now,
                    Title = "Not sold",
                    Message = $"Your card did not receive any bids and has been added back to your collection.",
                    Seen = false,
                    NotifyUser = sale.Seller,
                };
                //adds notification to database
                db.Notifications.Add(notifyUnsold);

                //adds card back into sellers collection since it did not sell
                sale.Seller.Collection.Add(sale.Card);

                //removes from potential watchers watchlists
                foreach (var i in sale.Watcher)
                {
                    i.Watchlist.Remove(sale);
                }

                //removes from potential users shoppingcarts
                foreach (var i in sale.Shopper)
                {
                    i.ShoppingCart.Remove(sale);
                }

                //removes the sale from the sales table in the database
                db.Sales.Remove(sale);
                //saves changes
                db.SaveChanges();
                //alerts users that an auction has just ended.
                return Json("Auction ended", JsonRequestBehavior.AllowGet);
            }

            //else create a notifcation for the winner of the auction
            Notification notify = new Notification()
            {
                Date = DateTime.Now,
                Title = "Auction won",
                Message = $"You have won the auction for {sale.Card.name} with the bid of ${bid[0].BidAmount}.",
                Seen = false,
                NotifyUser = bid[0].Bidder,
            };

            //sets the highest bidder as the buyer of the auction
            sale.Buyer = bid[0].Bidder;
            //adds the notification to the database
            db.Notifications.Add(notify);
            //changes the is sold property to true
            sale.IsSold = true;

            //notifies all users that did not win the auction about who won the auction, which card they won and for what price
            for (int i = 0; i < bid.Count; i++)
            {
                if (i != 0)
                {
                    //if a user bid the same ammount as the winner but were slower tell them how many seconds slower they were.
                    if (bid[0].BidAmount == bid[i].BidAmount)
                    {
                        //notifies all users with the same bid how many seconds they lost by
                        Notification notify2 = new Notification()
                        {
                            Date = DateTime.Now,
                            Title = "Auction lost",
                            Message = $"{bid[0].Bidder.Username} won {sale.Card.name} for ${bid[0].BidAmount} beating you by {(bid[i].TimeStamps - bid[0].TimeStamps).Seconds} seconds",
                            Seen = false,
                            NotifyUser = bid[i].Bidder,
                        };
                        //adds card to database
                        db.Notifications.Add(notify2);
                    }
                    else
                    {
                        //notifies users who they lost to and what he bid
                        Notification notify2 = new Notification()
                        {
                            Date = DateTime.Now,
                            Title = "Auction lost",
                            Message = $"{bid[0].Bidder.Username} won {sale.Card.name} for ${bid[0].BidAmount}.",
                            Seen = false,
                            NotifyUser = bid[i].Bidder,
                        };
                        //adds card to database
                        db.Notifications.Add(notify2);
                    }
                }
            }
            //saves changes to database
            db.SaveChanges();
            //tells users, auction ended
            return Json("Auction ended", JsonRequestBehavior.AllowGet);
        }

        //Returns the first card that's for auction that's also not sold.
        [HttpPost]
        public ActionResult AuctionNew()
        {
            Sale sale = db.Sales.Include("Card").Where(x => x.IsSold == false & x.ForAuction == true).FirstOrDefault();

            return Json(sale, JsonRequestBehavior.AllowGet);
        }
    }
}
