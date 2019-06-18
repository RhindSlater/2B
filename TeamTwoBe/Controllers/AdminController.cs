using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.ViewModels;

namespace TeamTwoBe.Controllers
{
    public class AdminsController : Controller
    {
        private Context db = new Context();

        public ActionResult CMS()
        {
            if(Session["UserID"] != null)
            {
                int id = Convert.ToInt32(Session["UserID"].ToString());
                User user = db.Users.Include("UserLevel").Where(x => x.ID == id).FirstOrDefault();
                if(user.UserLevel.ID == 1)
                {
                    AdminViewModel vm = new AdminViewModel()
                    {
                        Users = db.Users.ToList(),
                        Sales = db.Sales.Include("Card").Include("Buyer").Include("Seller").ToList(),
                        Verified = db.Sales.Include("Card").Include("Buyer").Include("Seller").Where(x => x.IsVerified == true).ToList(),
                        Bids = db.Bids.Include("Bidder").Include("Item.Card").ToList(),
                        notify = new Notification(),
                    };
                    return View(vm);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult notifyVerify(int id)
        {
            Sale sale = db.Sales.Find(id);
            User user = db.Users.Find(sale.Seller);
            Notification notify = new Notification()
            {
                Date = DateTime.Now,
                Title = "Sale Verified",
                Message = $"{sale.Card.name} has been successfully verified.",
                NotifyUser = user,
                Seen = false,
            };
            db.Notifications.Add(notify);
            return RedirectToAction("CMS");
        }
        public ActionResult notifyShipped(int id)
        {
            Sale sale = db.Sales.Find(id);
            User user = db.Users.Find(sale.Seller);
            Notification notify = new Notification()
            {
                Date = DateTime.Now,
                Title = "Sale Shipped",
                Message = $"{sale.Card.name} has been Shipped, you should receive it in the next coming days.",
                NotifyUser = user,
                Seen = false,
            };
            db.Notifications.Add(notify);
            return RedirectToAction("CMS");
        }

        public ActionResult Lock(int id)
        {
            User user = db.Users.Find(id);
            user.IsLocked = true;
            return View("CMS");
        }

        public ActionResult removeListing(int? id)
        {
            Sale sale = db.Sales.Include("Card").Include("Seller").Where(x => x.ID == id).FirstOrDefault();
            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removeListing(int id)
        {
            Sale sale = db.Sales.Include("Shopper").Include("Watcher").Include("Card").Include("Seller.Collection").Where(x => x.ID == id).FirstOrDefault();
            if (sale.IsSold == true)
            {
                return RedirectToAction("Index");
            }
            User user = db.Users.Find(sale.Seller.ID);

            foreach (var i in sale.Shopper)
            {
                Notification notify = new Notification()
                {
                    Title = "Sale removed",
                    Message = "A card in your shoppingcart has been unlisted.",
                    NotifyUser = i,
                    Seen = false,
                    Date = DateTime.Now
                };
                db.Notifications.Add(notify);
                i.ShoppingCart.Remove(sale);
            }

            foreach (var i in sale.Watcher)
            {
                Notification notify = new Notification()
                {
                    Title = "Sale removed",
                    Message = "A card on your watchlist has been unlisted.",
                    NotifyUser = i,
                    Seen = false,
                    Date = DateTime.Now
                };
                db.Notifications.Add(notify);
                i.Watchlist.Remove(sale);
            }

            //adds your unsold card to your collection
            user.Collection.Add(sale.Card);

            db.Sales.Remove(sale);
            db.SaveChanges();
            return RedirectToAction("CMS");
        }

        //edit user action
        public ActionResult Edit(int id)
        {
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
                return RedirectToAction("CMS");
            }
            return View(user);
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