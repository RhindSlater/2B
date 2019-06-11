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

        public ActionResult Index()
        {
            checkCookie();
            HomeViewModel vm = new HomeViewModel()
            {
                Followers = new List<Models.Sale>(),
                Trending = new List<Models.Sale>(),
                Recommended = new List<Models.Sale>(),
                UpcomingAuction = new List<Models.Sale>(),
                CurrentAuction = new Sale(),
            };
            if (Session["UserID"] != null)
            {
                if (Session["UserID"].ToString() != 0.ToString())
                {
                    int id = Convert.ToInt32(Session["UserID"].ToString());
                    User user = db.Users.Include("Follower").Where(x => x.ID == id).FirstOrDefault();
                    if (user.Follower.Count != 0)
                    {
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
                        while (vm.Followers.Count < 10)
                        {
                            vm.Followers.Add(db.Sales.Find(1));
                        }
                    }
                }
            }
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.IsSold == false & x.ForAuction == false))
            {
                vm.Trending.Add(i);
            }
            vm.Trending.Reverse();
            vm.Trending.RemoveRange(10, vm.Trending.Count - 10);

            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 3 & x.IsSold == false & x.ForAuction == false))
            {
                vm.Recommended.Add(i);
            }
            vm.Recommended.RemoveRange(10, vm.Recommended.Count - 10);

            vm.CurrentAuction = db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.ForAuction == true & x.IsSold == false).FirstOrDefault();

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

        public ActionResult WhyUs()
        {

            return View();

        }

        public ActionResult FAQ()
        {

            return View();

        }
    }
}