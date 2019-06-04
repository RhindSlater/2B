using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.ViewModels;
using TeamTwoBe.Views.ViewModels;

namespace TeamTwoBe.Controllers
{
    public class SalesController : Controller
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
            UsersController uc = new UsersController();
            List<Sale> li = new List<Sale>();
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 1 & x.IsSold == false & x.ForAuction == false))
            {
                li.Add(i);
            }
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 3 & x.IsSold == false & x.ForAuction == false))
            {
                li.Add(i);
            }
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 2 & x.IsSold == false & x.ForAuction == false))
            {
                li.Add(i);
            }
            if (Session["UserID"] == null)
            {
                Session["UserID"] = 0;
            }
            return View(li);
        }

        public ActionResult MyListings(int? id)
        {
            checkCookie();
            List<Sale> li = new List<Sale>();
            ListCardListSale vm = new ListCardListSale()
            {
                Sales = li,
                Users = new List<User>(),
            };
            if (Session["UserID"] == null)
            {
                Session["UserID"] = 0;
            }
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("Watcher").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.ID == id & x.IsSold == false & x.ForAuction == false))
            {
                li.Add(i);
                vm.Users.Add(i.Seller);
            }
            vm.Sales = li;
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> apiPrice(string dropboxvalue)
        {
            checkCookie();
            if (Session["userID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }
            ViewBag.Conditions = new SelectList(db.Conditions, "ID", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "ID", "Grading");

            HttpResponseMessage resp = await yugiohPriceApi.GetAsync($"get_card_prices/{dropboxvalue}");
            if (resp.IsSuccessStatusCode)
            {
                var rsp = await resp.Content.ReadAsStringAsync();

                var li = JObject.Parse(rsp).ToObject<RootObject>();

                SaleConditionGradeVM salevm = new SaleConditionGradeVM()
                {
                    fix = true,
                    MyCard = dropboxvalue,
                    MyDatum = new List<Datum>(),
                    MyCard1 = "",
                };

                foreach (var i in li.data)
                {
                    if (i.price_data.status == "success")
                    {
                        Card card = new Card()
                        {
                            apiID = $"{dropboxvalue} {i.print_tag} {i.rarity}",
                            name = dropboxvalue,
                            rarity = i.rarity,
                            print_tag = i.print_tag,
                            Cardtype = db.CardTypes.Find(1),
                            image_url = "http://www.ygo-api.com/api/Images/cards/" + dropboxvalue,
                            average = i.price_data.data.prices.average,
                            high = i.price_data.data.prices.high,
                            low = i.price_data.data.prices.low,
                        };
                        if (db.Cards.Where(x => x.name == dropboxvalue).FirstOrDefault() == null)
                        {
                            db.Cards.Add(card);
                        }
                        salevm.MyDatum.Add(i);
                    }
                }
                db.SaveChanges();
                Card cd = db.Cards.Where(x => x.name == dropboxvalue).FirstOrDefault();
                salevm.MyCard1 = cd.name;


                return View("Create", salevm);
            }
            SaleConditionGradeVM salev = new SaleConditionGradeVM();
            return View("Create", salev);
        }

        public ActionResult Details(int? id)
        {
            checkCookie();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session["View"] = "SaleDetail";
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        public async Task<ActionResult> Create(int? id)
        {
            checkCookie();
            ViewBag.Conditions = new SelectList(db.Conditions, "ID", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "ID", "Grading");

            //Stops anyone from creating a new sale if they are not logged in as a valid user. ~Joe
            if (Session["userID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Session["View"] = "SaleCreate";



            SaleConditionGradeVM salevm = new SaleConditionGradeVM();
            if (id != null)
            {
                Card card = db.Cards.Find(id);
                if (card != null)
                {
                    salevm.MyCard = card.name;
                }
            }
            HttpResponseMessage response = await yugiohApi.GetAsync($"v4/cardinfo.php?");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();
                List<Card> li2 = new List<Card>();
                foreach (var i in li)
                {
                    if (li2.Where(x => x.name == i.name).FirstOrDefault() == null)
                    {
                        li2.Add(i);
                    }
                }

                salevm.MyCards = li2;
                salevm.fix = false;
                return View(salevm);
            }
            salevm.fix = false;
            return View(salevm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Price,ForAuction")] SaleConditionGradeVM sale, string dropboxvalue, string Conditions, string Grades)
        {
            checkCookie();
            if (ModelState.IsValid)
            {
                Card card = db.Cards.Where(x => x.apiID == dropboxvalue).FirstOrDefault();
                Grade grade = db.Grades.Find(Convert.ToInt32(Grades));
                Condition condition = db.Conditions.Find(Convert.ToInt32(Conditions));
                User user = db.Users.Find(Session["UserID"]);

                Sale MySale = new Sale()
                {
                    Card = card,
                    Buyer = null,
                    CardCondition = condition,
                    CardGrade = grade,
                    ForAuction = sale.ForAuction,
                    ID = sale.ID,
                    Seller = user,
                    Price = sale.Price,
                    IsSold = false,
                    UploadDate = DateTime.Now.Date,
                };

                db.Sales.Add(MySale);
                var li = db.Users.Include("Watchlist").Include("Wishlist").Where(x => x.Wishlist.Any(z => z.ID == card.ID)).ToList();
                foreach (var i in li)
                {
                    Notification notify = new Notification()
                    {
                        Date = DateTime.Now,
                        Title = "New Sale",
                        Message = "A card in your wishlist is now for sale and has been added to your watchlist",
                        Seen = false,
                        NotifyUser = i,
                    };
                    db.Notifications.Add(notify);
                    i.Watchlist.Add(MySale);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sale);
        }

        [HttpPost]
        public ActionResult Search(string search)
        {
            checkCookie();

            if (search == null)
            {
                return RedirectToAction("Index");
            }
            ListCardListSale li = new ListCardListSale()
            {
                Cards = new List<Card>(),
                Sales = new List<Sale>(),
                Users = new List<User>(),
            };

            foreach (var i in db.Sales.Include("Card.Cardtype").Include("CardGrade").Include("CardCondition").Include("Watcher").Include("Seller.UserLevel").Where(x => x.Card.name.Contains(search) & x.ForAuction == false | x.Card.print_tag.Contains(search) & x.ForAuction == false | x.Card.Cardtype.Name == search & x.ForAuction == false | x.Seller.Username == search & x.ForAuction == false | x.CardGrade.Grading == search & x.ForAuction == false | x.Card.rarity.Contains(search) & x.ForAuction == false))
            {
                if (i.ID != 1 & i.IsSold == false)
                {
                    li.Sales.Add(i);
                }
            }
            foreach (var i in db.Cards.Include("Cardtype").Where(x => x.name.Contains(search) | x.print_tag.Contains(search) | x.Cardtype.Name == search | x.rarity.Contains(search)))
            {
                if (li.Cards.Where(x => x.name == i.name).FirstOrDefault() == null)
                {
                    if (i.ID != 1)
                    {
                        li.Cards.Add(i);
                    }
                }
            }
            foreach (var i in db.Users.Where(x => x.Username.Contains(search)))
            {
                if (i.ID != 1)
                {
                    li.Users.Add(i);
                }
            }
            return View(li);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editListing([Bind(Include = "ID,Price,ForAuction")] Sale sale, string Conditions, string Grades)
        {
            checkCookie();
            ViewBag.Conditions = new SelectList(db.Conditions, "ID", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "ID", "Grading");

            Sale sale1 = db.Sales.Include("Card").Include("Watcher").Include("Shopper").Include("Seller").Include("CardCondition").Include("CardGrade").Where(x => x.ID == sale.ID).AsNoTracking().FirstOrDefault();

            if (Conditions != "")
            {
                Condition condition = db.Conditions.Find(Convert.ToInt32(Conditions));
                sale1.CardCondition = condition;
            }
            if (Grades != "")
            {
                Grade grade = db.Grades.Find(Convert.ToInt32(Grades));
                sale1.CardGrade = grade;
            }
            if (sale1.Price != sale.Price)
            {
                foreach (var i in sale1.Watcher)
                {
                    Notification notify = new Notification()
                    {
                        Title = "Price changed",
                        Message = "A card on your watchlist has been edited.",
                        NotifyUser = i,
                        Seen = false,
                    };
                    db.Notifications.Add(notify);
                }
                foreach (var i in sale1.Shopper)
                {
                    Notification notify = new Notification()
                    {
                        Title = "Price changed",
                        Message = "A card in your shoppingcart has been edited.",
                        NotifyUser = i,
                        Seen = false,
                    };
                    db.Notifications.Add(notify);
                }
            }

            sale1.Price = sale.Price;
            sale1.ForAuction = sale.ForAuction;
            db.Entry(sale1).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult editListing(int? id)
        {
            checkCookie();
            ViewBag.Conditions = new SelectList(db.Conditions, "ID", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "ID", "Grading");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sale sale = db.Sales.Include("Seller").Include("Card").Where(x => x.ID == id).FirstOrDefault();
            if (sale == null)
            {
                return HttpNotFound();
            }

            //stops others from removing your sales
            if (sale.Seller.ID.ToString() == Session["UserID"].ToString())
            {
                Session["View"] = "SaleEdit";
                return View(sale);
            }
            return RedirectToAction("Login", "Users");
        }

        public ActionResult removeListing(int? id)
        {
            checkCookie();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sale sale = db.Sales.Include("Card").Include("Seller").Where(x => x.ID == id).FirstOrDefault();
            if (sale == null)
            {
                return HttpNotFound();
            }

            //stops others from removing your sales
            if (sale.Seller.ID.ToString() == Session["UserID"].ToString())
            {
                Session["View"] = "SaleDelete";
                return View(sale);
            }
            return RedirectToAction("Login", "Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removeListing(int id)
        {
            checkCookie();
            Sale sale = db.Sales.Include("Shopper").Include("Watcher").Include("Card").Include("Seller.Collection").Where(x => x.ID == id).FirstOrDefault();
            User user = db.Users.Find(sale.Seller.ID);

            foreach (var i in sale.Shopper)
            {
                Notification notify = new Notification()
                {
                    Title = "Sale removed",
                    Message = "A card in your shoppingcart has been unlisted.",
                    NotifyUser = i,
                    Seen = false,
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
                };
                db.Notifications.Add(notify);
                i.Watchlist.Remove(sale);
            }

            //adds your unsold card to your collection
            user.Collection.Add(sale.Card);

            db.Sales.Remove(sale);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Check if this id being parsed in from the Won view, is == to the ID in the Sale Table!
        public ActionResult ReviewSale(int? id)
        {
            checkCookie();

            if(id == null)
            {
                return RedirectToAction("MyReviews");
            }

            Sale sale = db.Sales.Include("Buyer").Include("Seller").Include("Card").Where(x => x.ID == id).FirstOrDefault();

            User user = db.Users.Find(Session["UserID"]);



            UserReview userReview = new UserReview()
            {
                ID = sale.ID,
                Reviewer = sale.Buyer,
                Reviewee = sale.Seller,
                CardReviewed = sale,
            };

            return View(userReview);


        }
        [HttpPost]
        public ActionResult ReviewSale(int id)
        {
            checkCookie();
            if (Session["userID"] == null)
            {
                return RedirectToAction("login", "users");
            }
            if (Session["UserID"].ToString() == "0")
            {
                return RedirectToAction("login", "users");
            }

            Sale sale = db.Sales.Include("Buyer").Include("Seller").Include("Card").Where(x => x.ID == id).FirstOrDefault();

            UserReview userReview = new UserReview()
            {
                Reviewer = sale.Buyer,
                Reviewee = sale.Seller,
                //This sets the sale ID as well.
                CardReviewed = sale,
            };

            if (userReview != null)
            {
                db.UserReviews.Add(userReview);
                Notification notify = new Notification()
                {
                    Date = DateTime.Now,
                    Message = $"{sale.Buyer.Username} has reviewed your {sale.Card.name}!",
                    Title = "Sale Reviewed",
                    NotifyUser = sale.Seller,
                    Seen = false,
                };
                db.Notifications.Add(notify);
                db.SaveChanges();
                return RedirectToAction("MyReviews");
            }
            else
            {
                return View("Won","Profile");
            }
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
