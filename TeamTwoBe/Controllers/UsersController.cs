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
            if(user.ID != 0)
            {
                user = db.Users.Find(user.ID);
                AccountType ACL = db.AccountTypes.Find(1);
                if (user.UserLevel == ACL)
                {
                    return View(db.Users.ToList());
                }
            }
            return RedirectToAction("Index", "Sales");
        }

        public ActionResult Login(User user)
        {
            if (user.Password != null)
            {
                User newUser = db.Users.SingleOrDefault(x => x.Username == user.Username);
                if(newUser != null)
                {
                    bool test = Crypto.VerifyHashedPassword(newUser.Password, user.Password);
                    if (test)
                    {
                        Session["UserID"] = newUser.ID;
                        Session["UserPic"] = newUser.DisplayPicture;

                        if (newUser.IsLocked)
                        {
                            RedirectToAction("Locked");
                        }
                        return RedirectToAction($"Profile/{newUser.ID}"); //change to home page once we have created one
                    }
                }
            }
            return View();

        }

        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "ID,FirstName,LastName,Username,Password,City,Email,Phone")] User user)
        {
            if (ModelState.IsValid)
            {
                if(db.Users.Where(x => x.Username == user.Username).Count() > 0)
                {
                    throw new Exception("This username is taken. Please select another one");
                }
                else
                {
                    user.Password = Crypto.HashPassword(user.Password);
                    AccountType ACL = db.AccountTypes.Find(2);
                    user.UserLevel = ACL;
                    user.DisplayPicture = "Default.png";
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
                User user2 = db.Users.Where(x => x.ID == user.ID).AsNoTracking().FirstOrDefault();
                if(user.Password != user2.Password)
                {
                    user2.Password = Crypto.HashPassword(user.Password);
                }
                db.Entry(user2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction($"Details/{user2.ID}");
            }
            return View(user);
        }


        //public ActionResult Profile(User LoggedInUser) // any profile
        //{
        //    if (Session["UserID"] == LoggedInUser)
        //    {
        //        return View(); //my profile
        //    }
        //    return View(); //someone elses profile
        //}
        public ActionResult Profile(int? id) // Logged in and looking at your home page
        {
            User LoggedInUser = db.Users.Find(id);
            if (Session["UserID"] == LoggedInUser)
            {
                return View(LoggedInUser); //my home page
            }
            return View(LoggedInUser); //home page
        }

        public ActionResult LogOut() // logged out
        {
            Session["UserID"] = null;
            return RedirectToAction("Index","Home");
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

        public ActionResult UserReview()
        {

            return View();
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
