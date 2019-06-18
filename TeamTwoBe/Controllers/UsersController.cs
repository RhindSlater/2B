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

namespace TeamTwoBe.Controllers
{
    public class UsersController : Controller
    {
        private Context db = new Context();

        //checks the ammount of sales in your shoppingcart
        public ActionResult CheckShoppingCount()
        {
            //checks if you have saved your cookies
            checkCookie();

            //finds your user id to and finds your user in the database
            int id = Convert.ToInt32(Session["UserID"].ToString());
            //includes the shoppingcart table
            User user = db.Users.Include("ShoppingCart").Where(x => x.ID == id).FirstOrDefault();
            //returns the ammount of cards in your shopping cart
            id = user.ShoppingCart.Count();
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        //Checks if a user is logged in or not
        public void CheckUserID()
        {
            if (Session["UserID"] == null)
            {
                RedirectToAction("Login");
                return;
            }
            if (Session["UserID"].ToString() == "0")
            {
                RedirectToAction("Login");
                return;
            }
        }


        //Used to check if a password entered matches their hashed password in the database
        [HttpPost]
        public ActionResult PasswordVerify(string Password, int id)
        {
            checkCookie();
            CheckUserID();

            Sale sale = db.Sales.Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Where(x => x.ID == id).FirstOrDefault();
            if (Crypto.VerifyHashedPassword(user.Password, Password))
            {
                //if the passwords match then purchase the card
                return RedirectToAction("purchaseCard", "Profile", new { id = sale.ID });
            }
            else
            {
                //else return to shoppingcart
                return View("ShoppingCart", "Profile");
            }
        }
        //Used to check if a password entered matches their hashed password in the database
        [HttpPost]
        public ActionResult PasswordVerify2(string Password, int id)
        {
            checkCookie();
            CheckUserID();

            Sale sale = db.Sales.Include("Shopper").Include("Watcher.Watchlist").Include("Seller").Include("Card").Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Where(x => x.ID == id).FirstOrDefault();
            if (user != null)
            {
                if (Crypto.VerifyHashedPassword(user.Password, Password))
                {
                    sale.IsSold = true;
                    sale.IsVerified = true;
                    sale.Buyer = user;

                    foreach (var i in sale.Shopper)
                    {
                        i.ShoppingCart.Remove(sale);
                    }

                    foreach (var i in sale.Watcher)
                    {
                        i.Watchlist.Remove(sale);
                        Notification notify2 = new Notification()
                        {
                            Date = DateTime.Now,
                            Title = "Card Sold",
                            Message = $"{user.Username} has purchased {sale.Card.name} that was in your watchlist",
                            Seen = false,
                            NotifyUser = i,
                        };
                        db.Notifications.Add(notify2);
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
                    return RedirectToAction("Won", "Profile");
                }
                //else return to shoppingcart
                return View("ShoppingCart", "Profile");
            }
            else
            {
                //else return to shoppingcart
                return View("ShoppingCart", "Profile");
            }
        }

        //Checks the notification table for notifications that are aisigned to your user
        public ActionResult CheckNotifications()
        {
            int id = Convert.ToInt32(Session["UserID"].ToString());
            List<Notification> li = db.Notifications.Include("NotifyUser").Where(x => x.NotifyUser.ID == id).ToList();
            li.Reverse();
            //returns a list of notifications sorted from most recent to oldest
            return Json(li, JsonRequestBehavior.AllowGet);
        }

        //Changes the notification to seen when you click on it
        public ActionResult ChangeNotification(int id)
        {
            //finds your user id
            int id1 = Convert.ToInt32(Session["UserID"].ToString());
            //creates a list of all your notifications
            List<Notification> li = db.Notifications.Include("NotifyUser").Where(x => x.NotifyUser.ID == id1).ToList();
            //sorts from most recent to old
            li.Reverse();

            //checks if the notification has already been seen
            if (li[id].Seen == true)
            {
                //if it has return false.(No changes needed)
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            //else
            //looks for the notification using the id passed in from the view
            //as an index of the list of notifications to change to seen.
            li[id].Seen = true;
            //save changes
            db.SaveChanges();
            //return true and remove unseen properties
            return Json("true", JsonRequestBehavior.AllowGet);
        }


        public ActionResult Subscription()
        {
            checkCookie();
            CheckUserID();
            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("UserLevel").Where(x => x.ID == id).FirstOrDefault();
            Models.AccountType acc = db.AccountTypes.Find(3);
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

        //Shows the premium view
        public ActionResult Premium()
        {
            checkCookie();
            CheckUserID();

            int id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("UserLevel").Where(x => x.ID == id).FirstOrDefault();
            //finds your user and checks if you are a premium member along with dates and payment method
            PremiumBilling bill = db.PremiumBilling.Where(x => x.Member.ID == user.ID).FirstOrDefault();
            PremiumViewModel vm = new PremiumViewModel
            {
                MyUser = user,
                MyBilling = bill,
            };
            return View(vm);
        }

        //view login page
        public ActionResult Login()
        {
            //if your login cookies are saved login then redirect home
            var i = checkCookie();
            if (i)
            {
                return RedirectToAction("Index", "Home");
            }
            //null pointer
            if (Session["UserID"] != null)
            {
                //if user is logged in return home
                if (Convert.ToInt32(Session["UserID"].ToString()) >= Convert.ToInt32("1"))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            //else return to login page
            //needed to stop the layout from loading on the login page
            Session["View"] = "loginpage";
            return View();
        }

        //Checks if a user has previously saved their cookies
        public bool checkCookie()
        {
            string userid = string.Empty;
            if (Request != null)
            {
                //checks the users cookies for userid
                if (Request.Cookies["userid"] != null)
                {
                    //if cookies found bind userid value to variable
                    userid = Request.Cookies["userid"].Value;
                    //search database for a user with the same cookie saved
                    User user = db.Users.Include("UserLevel").Include("ShoppingCart").Where(x => x.cookie == userid).FirstOrDefault();

                    //if user exists, log them in and save data in session
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


        //login view
        [HttpPost]
        public ActionResult Login(string Username, string Password, dynamic SaveData)
        {
            //Checks if the user entered a password
            if (Password != "")
            {
                //look for the user with the entered unsername
                User user = db.Users.Include("ShoppingCart").Include("UserLevel").SingleOrDefault(x => x.Username == Username);
                //checks if user exists
                if (user != null)
                {
                    //verify's if the users password matches the hashed password in our database
                    if (Crypto.VerifyHashedPassword(user.Password, Password))
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

        //Register view
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



        //Creates the user and adds to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "ID,FirstName,LastName,Username,Password,City,Email,Phone")] User user)
        {
            if (ModelState.IsValid)
            {
                //if username is taken
                if (db.Users.Where(x => x.Username == user.Username).Count() > 0)
                {
                    throw new Exception("This username is taken. Please select another one");
                }
                else if (db.Users.Where(x => x.Email == user.Email).Count() > 0)
                {
                    throw new Exception("This email is taken. Please select another one");
                }
                else
                {
                    //has the entered password and save it in the database
                    user.Password = Crypto.HashPassword(user.Password);
                    //set user level as a guest user
                    AccountType ACL = db.AccountTypes.Find(2);
                    user.UserLevel = ACL;
                    //sets the default display picture
                    user.DisplayPicture = "Default.png";
                    user.Follower = new List<User>();
                    user.Following = new List<User>();
                    user.Wishlist = new List<Card>();
                    user.ShoppingCart = new List<Sale>();
                    user.Collection = new List<Card>();
                    user.IsDeleted = false;
                    user.IsLocked = false;
                    //adds user to database
                    db.Users.Add(user);

                    //creates notification asking you to verify your email
                    Notification notify = new Notification()
                    {
                        Date = DateTime.Now,
                        Title = "Registration",
                        Message = "Your account has successfully been created. Please verify your email.",
                        Seen = false,
                        NotifyUser = user,
                    };
                    //add notification to database
                    db.Notifications.Add(notify);
                    //save changes
                    db.SaveChanges();
                    return RedirectToAction("Login", new { Username = user.Username });
                }
            }
            return View(user);
        }

        //show only your user details
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

        //edit user action
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


        //edit user action(Save changes)
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

        //Load profile and current auctions
        public ActionResult Profile(int? id) // Logged in and looking at your home page
        {
            checkCookie();

            //if session is null and id
            if (id == null & Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Sales");
            }
            else if (id == null)
            {
                //if id == null set id to session 
                if (Convert.ToInt32(Session["UserID"].ToString()) == 0)
                {
                    return RedirectToAction("Index", "Sales");
                }
                id = Convert.ToInt32(Session["UserID"].ToString());
            }
            else if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if(Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("Login", "Users");
            }
            User user = db.Users.Include("Collection.Cardtype").Include("Following").Include("Wishlist.Cardtype").Include("Watchlist.Seller").Include("Watchlist.CardCondition").Include("Watchlist.CardGrade").Include("Watchlist.Card.Cardtype").Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user2 = db.Users.Include("Collection.Cardtype").Include("Wishlist.Cardtype").Include("Watchlist.Seller").Include("Watchlist.CardCondition").Include("Watchlist.CardGrade").Include("Watchlist.Card.Cardtype").Where(x => x.ID == id).FirstOrDefault();
            ProfileViewModel vm = new ProfileViewModel()
            {
                MyCollection = new List<Card>(),
                MyWatchList = new List<Sale>(),
                MyWishList = new List<Card>(),
                MySales = new List<Sale>(),
                MyUser = user,
                LoggedInUser = user2,
            };
            List<Sale> li = new List<Sale>();

            //adds a list of cards(Max of 10) to the collection, wishlist and trending that meet the criteria
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("CardCondition").Include("CardGrade").Where(x => x.Seller.ID == user.ID & x.IsSold == false & x.ForAuction == false))
            {
                li.Add(i);
            }
            for (int i = 1; i < 11; i++)
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

        [HttpPost]
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

        //delete view
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

        //Delete a user
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
        public ActionResult Follow(int id)
        {
            checkCookie();
            //User user is the person you want to follow.
            User user = db.Users.Where(x => x.ID == id).FirstOrDefault();
            //id is now the current user's ID (you).
            id = Convert.ToInt32(Session["UserID"].ToString());
            //user2 is you.
            User user2 = db.Users.Include("Follower").Where(x => x.ID == id).FirstOrDefault();
            //If user2 is null, you need to login.
            if (user2 == null)
            {
                //Tells user to log in using AJAX.
                return Json("Log in to follow a user.", JsonRequestBehavior.AllowGet);
            }
            //if YOU do not follow the user.
            if (user2.Follower.Contains(user) == false)
            {
                //Create a new notification telling the user you followed them.
                Notification notify = new Notification()
                {
                    Date = DateTime.Now,
                    Title = "New Follower",
                    Message = user2.Username + " has started following you.",
                    Seen = false,
                    NotifyUser = user,
                };
                //follows the user.
                user2.Follower.Add(user);
                //Adds notification to the database.
                db.Notifications.Add(notify);
                //Saves the changes in the database.
                db.SaveChanges();
                //Uses AJAX to notify you follow that user (including their username).
                return Json("You have successfully followed " + user.Username, JsonRequestBehavior.AllowGet);
            }
            //Else, this will remove them from your followings.
            user2.Follower.Remove(user);
            //Saves the changes to the database.
            db.SaveChanges();
            //Else, returns AJAX message to notify you that you unfollowed that user.
            return Json("You have successfully unfollowed " + user.Username, JsonRequestBehavior.AllowGet);
        }


        //Sends a notification to a user that has a card that you want to trade for
        public ActionResult requestTrade(int id)
        {

            Card card = db.Cards.Include("CollectionOwners").Where(x => x.ID == id).FirstOrDefault();
            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Where(x => x.ID == id).FirstOrDefault();
            if (card != null)
            {
                //send a notification to every user that has the card you want to trade for
                foreach (var i in card.CollectionOwners)
                {
                    Notification notify = new Notification()
                    {
                        Date = DateTime.Now,
                        Title = "Trade request",
                        Message = user.Username + " has requested to trade for the " + card.name + " in your collection.",
                        Seen = false,
                        NotifyUser = i,
                    };
                    db.Notifications.Add(notify);
                }
                //save changes
                db.SaveChanges();
                return Json("Your request has been sent.", JsonRequestBehavior.AllowGet);
            }
            //if card does not exist or card is not in anyones shoppingcart return this
            return Json("No users currently have selected card in their collection", JsonRequestBehavior.AllowGet);
        }

    }
}
