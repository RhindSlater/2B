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


namespace TeamTwoBe.Controllers
{
    public class UsersController : Controller
    {
        private Context db = new Context();

        // GET: Users
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

        public ActionResult Login(User user)
        {
            if (Session["UserID"] != null)
            {
                if (Convert.ToInt32(Session["UserID"].ToString()) > Convert.ToInt32("1"))
                {
                    return RedirectToAction("Index");
                }
            }

            if (user.Password != null)
            {
                User newUser = db.Users.SingleOrDefault(x => x.Username == user.Username);
                if (newUser != null)
                {
                    bool test = Crypto.VerifyHashedPassword(newUser.Password, user.Password);
                    if (test)
                    {
                        Session["UserID"] = newUser.ID;
                        Session["Username"] = newUser.Username;
                        Session["UserPic"] = newUser.DisplayPicture;

                        if (newUser.IsLocked)
                        {
                            return RedirectToAction("Locked");
                        }
                        return RedirectToAction($"Profile/{newUser.ID}"); //change to home page once we have created one
                    }
                }
            }
            Session["View"] = "UserLogin";
            return View();

        }

        public ActionResult Register()
        {
            if(Session["UserID"] == null)
            {
                return View();
            }
            if(Convert.ToInt32(Session["UserID"].ToString()) > Convert.ToInt32("1"))
            {
                return RedirectToAction("Index");
            }
            Session["View"] = "UserRegister";
            return View();
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
                    user.IsDeleted = false;
                    user.IsLocked = false;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }

            return View(user);
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
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
            Session["View"] = "UserDetails";
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
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
            Session["View"] = "UserEdit";
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
            Session["View"] = "UserProfile";
            if(id == null)
            {
                return RedirectToAction("Index","Users");
            }
            if (id == Convert.ToInt32(Session["UserID"].ToString()))
            {
                User user = db.Users.Find(Session["UserID"]);
                return View(user);
            }
            else
            {
                //other users profiles
                User user = db.Users.Find(id);
                return View(user);
            }
        }

        public ActionResult LogOut() // logged out
        {
            Session["UserID"] = null;
            return RedirectToAction("Index", "Home");
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
    }
}
