using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.ViewModels;

namespace TeamTwoBe.Controllers
{

    public class HomeController : Controller
    {
        private Context db = new Context();

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

        //home page view
        public ActionResult Index()
        {
            checkCookie();
            //creates homepage viewmodel 
            HomeViewModel vm = new HomeViewModel()
            {
                Followers = new List<Models.Sale>(),
                Trending = new List<Models.Sale>(),
                Recommended = new List<Models.Sale>(),
                UpcomingAuction = new List<Models.Sale>(),
                CurrentAuction = new Sale(),
            };
            //null pointer
            if (Session["UserID"] != null)
            {
                if (Session["UserID"].ToString() != 0.ToString())
                {
                    //finds user id
                    int id = Convert.ToInt32(Session["UserID"].ToString());
                    //finds the user
                    User user = db.Users.Include("Follower").Where(x => x.ID == id).FirstOrDefault();
                    //checks if the user is following anyone
                    if (user.Follower.Count != 0)
                    {
                        //adds all sales to a list where the seller is someone you follow
                        foreach (var i in user.Follower)
                        {
                            foreach (var y in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.ID == i.ID & x.ForAuction == false & x.IsSold == false))
                            {
                                if (vm.Followers.Count == 10)
                                {
                                    break;
                                }
                                vm.Followers.Add(y);
                            }
                        }
                        //if there's less then 10 cards, add a blank card
                        while (vm.Followers.Count < 10)
                        {
                            vm.Followers.Add(db.Sales.Include("Card").Where(x => x.ID == 1).FirstOrDefault());
                        }
                    }
                }
            }
            //Show the 10 most recently uploaded cards
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.IsSold == false & x.ForAuction == false))
            {
                vm.Trending.Add(i);
            }
            vm.Trending.Reverse();
            vm.Trending.RemoveRange(10, vm.Trending.Count - 10);

            //Show 10 cards from premium members
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 3 & x.IsSold == false & x.ForAuction == false))
            {
                vm.Recommended.Add(i);
            }
            //removes extra cards from list
            vm.Recommended.RemoveRange(10, vm.Recommended.Count - 10);

            //finds the next card to be auctioned off
            vm.CurrentAuction = db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.ForAuction == true & x.IsSold == false).FirstOrDefault();

            //finds the next 10 cards to be auctioned
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.IsSold == false & x.ForAuction == true))
            {
                vm.UpcomingAuction.Add(i);
            }
            while (vm.UpcomingAuction.Count < 11)
            {
                vm.UpcomingAuction.Add(db.Sales.Find(1));
            }
            vm.UpcomingAuction.Remove(vm.CurrentAuction);
            vm.UpcomingAuction.RemoveRange(10, vm.UpcomingAuction.Count - 10);

            return View(vm);
        }

        //why us view
        public ActionResult WhyUs()
        {
            return View();
        }

        //faq page
        public ActionResult FAQ()
        {
            return View();
        }
    }
}