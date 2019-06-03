using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using System.Web.Helpers;
using TeamTwoBe.ViewModels;
using System.Security.Cryptography;

namespace TeamTwoBe.Controllers
{
    public class UsersController : Controller
    {
        private Context db = new Context();

        public ActionResult Index(User user)
        {
            if (user.ID != 0)
            {
                user = db.Users.Find(user.ID);
                AccountType ACL = db.AccountTypes.Find(1);
                if (user.UserLevel == ACL)
                {
                    Session["View"] = "UserIndex";
                    return View(db.Users.ToList());
                }
            }
            return RedirectToAction("Index", "Sales");
        }
        public ActionResult CheckNotifications()
        {
            int id = Convert.ToInt32(Session["UserID"].ToString());
            List<Notification> li = db.Notifications.Include("NotifyUser").Where(x => x.NotifyUser.ID == id).ToList();
            li.Reverse();
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeNotification(int id)
        {
            int id1 = Convert.ToInt32(Session["UserID"].ToString());
            List<Notification> li = db.Notifications.Include("NotifyUser").Where(x => x.NotifyUser.ID == id1).ToList();
            if(li[id].Seen == true)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            li[id].Seen = true;
            db.SaveChanges();

            return Json("true",JsonRequestBehavior.AllowGet);
        }

        public ActionResult Subscription(string test)
        {
            checkCookie();
            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("UserLevel").Where(x => x.ID == id).FirstOrDefault();
            AccountType acc = db.AccountTypes.Find(3);
            user.UserLevel = acc;
            Session["AccountLevel"] = user.UserLevel.ID.ToString();
            PremiumBilling pb = new PremiumBilling()
            {
                Amount = 11.99,
                Date = DateTime.Now,
                Member = user,
                NextBillingDate = DateTime.Now.AddMonths(1)
            };
            Notification notify = new Notification()
            {
                Date = DateTime.Now,
                Title = "Subscription",
                Message = "Congrats, you're now a premium member here at 2B.",
                Seen = false,
                NotifyUser = user,
            };
            user.DisplayPicture = "premium.png";
            Session["UserPic"] = user.DisplayPicture;
            db.PremiumBilling.Add(pb);
            db.Notifications.Add(notify);
            db.SaveChanges();
            return RedirectToAction("Premium");
        }

        public ActionResult Premium()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("Login");
            }
            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("UserLevel").Where(x => x.ID == id).FirstOrDefault();
            PremiumBilling bill = db.PremiumBilling.Where(x => x.Member.ID == user.ID).FirstOrDefault();
            PremiumViewModel vm = new PremiumViewModel
            {
                MyUser = user,
                MyBilling = bill,
            };
            return View(vm);
        }

        public ActionResult Login()
        {
            var i = checkCookie();
            if (i)
            {
                return RedirectToAction("Index", "Sales");
            }
            if (Session["UserID"] != null)
            {
                if (Convert.ToInt32(Session["UserID"].ToString()) >= Convert.ToInt32("1"))
                {
                    return RedirectToAction("Index");
                }
            }
            Session["View"] = "loginpage";
            return View();
        }

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
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpPost]
        public ActionResult Login(string Username, string Password, dynamic SaveData)
        {
            if (Password != "")
            {
                User user = db.Users.Include("ShoppingCart").Include("UserLevel").SingleOrDefault(x => x.Username == Username);
                if (user != null)
                {
                    bool test = Crypto.VerifyHashedPassword(user.Password, Password);
                    if (test)
                    {
                        Session["UserID"] = user.ID;
                        Session["Username"] = user.Username;
                        Session["UserPic"] = user.DisplayPicture;
                        Session["ShoppingCart"] = user.ShoppingCart.Count();
                        Session["AccountLevel"] = user.UserLevel.ID.ToString();
                        if (user.IsLocked)
                        {
                            return RedirectToAction("Locked");
                        }
                        try
                        {
                            if (SaveData[0] == "on")
                            {
                                string str = user.Username + DateTime.Now;
                                HttpCookie cookie = new HttpCookie("userid");
                                cookie.Expires = DateTime.Now.AddDays(30);
                                cookie.Value = str;
                                Response.Cookies.Add(cookie);
                                user.cookie = str;
                                db.SaveChanges();
                            }
                            return RedirectToAction($"Profile/{user.ID}"); //change to home page once we have created one
                        }
                        catch (Exception)
                        {
                            return RedirectToAction($"Profile/{user.ID}"); //change to home page once we have created one
                        }
                    }
                }
            }
            Session["View"] = "loginpage";
            return View();
        }

        public ActionResult Register()
        {
            if (Session["UserID"] == null)
            {
                Session["View"] = "UserRegister";
                return View();
            }
            if (Session["UserID"].ToString() == "0")
            {
                Session["View"] = "UserRegister";
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "ID,FirstName,LastName,Username,Password,City,Email,Phone")] User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Where(x => x.Username == user.Username).Count() > 0)
                {
                    throw new Exception("This username is taken. Please select another one");
                }
                else
                {
                    user.Password = Crypto.HashPassword(user.Password);
                    AccountType ACL = db.AccountTypes.Find(2);
                    user.UserLevel = ACL;
                    user.DisplayPicture = "Default.png";
                    user.Follower = new List<User>();
                    user.Following = new List<User>();
                    user.Wishlist = new List<Card>();
                    user.ShoppingCart = new List<Sale>();
                    user.Collection = new List<Card>();
                    user.IsDeleted = false;
                    user.IsLocked = false;
                    db.Users.Add(user);
                    Notification notify = new Notification()
                    {
                        Date = DateTime.Now,
                        Title = "Registration",
                        Message = "Your account has successfully been created. Please verify your email.",
                        Seen = false,
                        NotifyUser = user,
                    };
                    db.Notifications.Add(notify);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View(user);
        }

        public ActionResult Details()
        {
            checkCookie();
            User user = db.Users.Find(Session["UserID"]);
            if (user == null)
            {
                return HttpNotFound();
            }
            Session["View"] = "UserDetails";
            return View(user);
        }

        public ActionResult Edit()
        {
            checkCookie();
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }
            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Find(id);
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Username,Password,City,Email,Phone")] User user)
        {
            if (ModelState.IsValid)
            {
                AccountType ACL = db.AccountTypes.Find(1);
                User user2 = db.Users.Where(x => x.ID == user.ID).AsNoTracking().FirstOrDefault();
                if (user.Password != user2.Password)
                {
                    user.Password = Crypto.HashPassword(user.Password);
                }
                user.DisplayPicture = user2.DisplayPicture;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                Session["Username"] = user.Username;
                return RedirectToAction($"Details/{user2.ID}");
            }
            return View(user);
        }

        public ActionResult Profile(int? id) // Logged in and looking at your home page
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Sales");
            }
            checkCookie();
            User user = db.Users.Include("Collection.Cardtype").Include("Wishlist.Cardtype").Include("Watchlist.Seller").Include("Watchlist.CardCondition").Include("Watchlist.CardGrade").Include("Watchlist.Card.Cardtype").Where(x => x.ID == id).FirstOrDefault();
            ProfileViewModel vm = new ProfileViewModel()
            {
                MyCollection = new List<Card>(),
                MyWatchList = new List<Sale>(),
                MyWishList = new List<Card>(),
                MySales = new List<Sale>(),
                MyUser = user,
            };
            List<Sale> li = new List<Sale>();

            foreach (var i in db.Sales.Include("Card.Cardtype").Include("CardCondition").Include("CardGrade").Where(x => x.Seller.ID == user.ID & x.IsSold == false))
            {
                li.Add(i);
            }
            for (int i = 1; i < 7; i++)
            {
                if (user.Collection.Count >= i)
                {
                    vm.MyCollection.Add(user.Collection[i - 1]);
                }
                else
                {
                    vm.MyCollection.Add(db.Cards.Find(1));
                }
                if (user.Wishlist.Count >= i)
                {
                    vm.MyWishList.Add(user.Wishlist[i - 1]);
                }
                else
                {
                    vm.MyWishList.Add(db.Cards.Find(1));
                }
                if (user.Watchlist.Count >= i)
                {
                    vm.MyWatchList.Add(user.Watchlist[i - 1]);
                }
                else
                {
                    vm.MyWatchList.Add(db.Sales.Find(1));
                }
                if (li.Count >= i)
                {
                    vm.MySales.Add(li[i - 1]);
                }
                else
                {
                    vm.MySales.Add(db.Sales.Find(1));
                }
            }
            return View(vm);
        }

        public ActionResult LogOut() // logged out
        {
            Session["UserID"] = null;
            Session["ShoppingCart"] = null;
            Session["AccountLevel"] = null;
            if (Response != null)
            {
                if (Response.Cookies["userid"] != null)
                {
                    HttpCookie cookie = new HttpCookie("userid");
                    cookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(cookie);
                }
            }
            return RedirectToAction("Index", "Sales");
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            Session["View"] = "UserDelete";
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            user.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult UserReview()
        {
            checkCookie();
            return View();
        }

        public ActionResult Follow(int id)
        {
            checkCookie();
            User user = db.Users.Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user2 = db.Users.Include("Follower").Where(x => x.ID == id).FirstOrDefault();
            if(user2 == null)
            {
                return Json("Log in to follow a user.", JsonRequestBehavior.AllowGet);
            }
            if (user2.Follower.Contains(user) == false)
            {
                Notification notify = new Notification()
                {
                    Date = DateTime.Now,
                    Title = "New Follower",
                    Message = user2.Username + " has started following you.",
                    Seen = false,
                    NotifyUser = user,
                };
                user2.Follower.Add(user);
                db.Notifications.Add(notify);
                db.SaveChanges();
                return Json("You have successfully followed " + user.Username, JsonRequestBehavior.AllowGet);
            }
            return Json("You already follow " + user.Username, JsonRequestBehavior.AllowGet);
        }

        public ActionResult requestTrade(int id)
        {
            Sale sale = db.Sales.Include("Seller").Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Where(x => x.ID == id).FirstOrDefault();
            if (sale != null)
            {
                Notification notify = new Notification()
                {
                    Date = DateTime.Now,
                    Title = "Trade request",
                    Message = user.Username + " has requested to trade for the " + sale.Card.name + " in your collection.",
                    Seen = false,
                    NotifyUser = sale.Seller,
                };
            }
            return Json("Your request has been sent.", JsonRequestBehavior.AllowGet);
        }

    }
}
