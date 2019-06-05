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

        [HttpPost]
        public ActionResult placebid(float id)
        {
            checkCookie();
            if (Session["UserID"] != null)
            {
                int id1 = Convert.ToInt32(Session["UserID"].ToString());
                User user = db.Users.Where(x => x.ID == id1).FirstOrDefault();
                if (user != null)
                {
                    Bid bid = new Bid()
                    {
                        TimeStamps = DateTime.Now,
                        BidAmount = id,
                        Bidder = user,
                        Item = db.Sales.Where(x => x.IsSold == false & x.ForAuction == true).FirstOrDefault(),
                    };
                    db.Bids.Add(bid);
                    db.SaveChanges();
                    return Json("Your bid has been placed", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Please login", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Please login", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AuctionEnd()
        {
            Sale sale = db.Sales.Include("Card").Where(x => x.ForAuction == true & x.IsSold == false).FirstOrDefault();
            sale.IsSold = true;

            List<Bid> bid = db.Bids.Include("Bidder").Where(x => x.Item.ID == sale.ID).OrderBy(x => x.BidAmount).ToList();

            Notification notify = new Notification()
            {
                Date = DateTime.Now,
                Title = "Auction won",
                Message = $"You have won the auction for {sale.Card.name} with the bid of ${bid[0].BidAmount}.",
                Seen = false,
                NotifyUser = bid[0].Bidder,
            };
            sale.Buyer = bid[0].Bidder;
            db.Notifications.Add(notify);

            for (int i = 0; i < bid.Count; i++)
            {
                if (i != 0)
                {
                    if (bid[0].BidAmount == bid[i].BidAmount)
                    {
                        Notification notify2 = new Notification()
                        {
                            Date = DateTime.Now,
                            Title = "Auction lost",
                            Message = $"{bid[0].Bidder} won the {sale.Card.name} for ${bid[0].BidAmount} beating you by {bid[0].TimeStamps - bid[i].TimeStamps}",
                            Seen = false,
                            NotifyUser = bid[i].Bidder,
                        };
                        db.Notifications.Add(notify2);
                    }
                    else
                    {
                        Notification notify2 = new Notification()
                        {
                            Date = DateTime.Now,
                            Title = "Auction lost",
                            Message = $"{bid[0].Bidder} won the {sale.Card.name} for ${bid[0].BidAmount}.",
                            Seen = false,
                            NotifyUser = bid[i].Bidder,
                        };
                        db.Notifications.Add(notify2);
                    }
                }
            }

            return Json("Auction ended", JsonRequestBehavior.AllowGet);
        }
    }
}
