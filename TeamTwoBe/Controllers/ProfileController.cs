using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        public ActionResult Shoppingcart()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            int id = Convert.ToInt32(Session["UserID"].ToString());

            User user = db.Users.Include("ShoppingCart.Card.CardType").Include("ShoppingCart.Seller").Include("ShoppingCart.CardGrade").Include("ShoppingCart.CardCondition").Where(x => x.ID == id).FirstOrDefault();
            return View(user);

        }

        public ActionResult Watchlist()
        {
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

        public ActionResult removeWishlist(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Card card = db.Cards.Where(x => x.ID == id).FirstOrDefault();

            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Wishlist").Where(x => x.ID == id).FirstOrDefault();
            user.Wishlist.Remove(card);
            db.SaveChanges();


            return RedirectToAction("Wishlist", new { id = id });
        }

        public ActionResult removeCollection(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Card card = db.Cards.Where(x => x.ID == id).FirstOrDefault();

            id = Convert.ToInt32(Session["UserID"].ToString());
            User user = db.Users.Include("Collection").Where(x => x.ID == id).FirstOrDefault();
            user.Collection.Remove(card);
            db.SaveChanges();


            return RedirectToAction("Collection", new { id = id });
        }

        public ActionResult removeFromWatchlist(int id)
        {
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

            return RedirectToAction("ShoppingCart");
        }

        public ActionResult addShoppingcart(int id)
        {
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
            User user = db.Users.Include("ShoppingCart").Include("Watchlist").Where(x => x.ID == id).FirstOrDefault();

            if (user.ShoppingCart.Contains(sale) == false)
            {
                user.ShoppingCart.Add(sale);
                db.SaveChanges();
            }
            if (user.Watchlist.Contains(sale) == false)
            {
                user.Watchlist.Add(sale);
                db.SaveChanges();
            }
            return RedirectToAction("Shoppingcart");
        }

        public ActionResult addToWatchlist(int id)
        {
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

            if(user.Watchlist.Contains(sale) == false)
            {
                user.Watchlist.Add(sale);
                db.SaveChanges();
            }
            return RedirectToAction("Watchlist");
        }

        public ActionResult Wishlist(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            User user = db.Users.Include("Wishlist.CardType").Where(x => x.ID == id).FirstOrDefault();
            if(user.Wishlist.Count == 0)
            {
                return RedirectToAction("Wishlist","Card", user.ID);
            }
            return View(user);
        }

        public ActionResult Collection(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            User user = db.Users.Include("Collection.CardType").Where(x => x.ID == id).FirstOrDefault();
            if (user.Collection.Count == 0)
            {
                return RedirectToAction("Collection", "Card", user.ID);
            }
            return View(user);
        }
    }
}