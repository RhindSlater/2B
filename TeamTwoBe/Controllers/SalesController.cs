﻿using Newtonsoft.Json.Linq;
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

        public ActionResult Index()
        {
            Session["View"] = "SaleIndex";
            List<Sale> li = new List<Sale>();
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 1))
            {
                li.Add(i);

            }
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 3))
            {
                li.Add(i);
            }
            foreach (var i in db.Sales.Include("Card.Cardtype").Include("CardCondition").Include("CardGrade").Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 2))
            {
                li.Add(i);
            }
            if(Session["UserID"] == null)
            {
                Session["UserID"] = 0;
            }
            return View(li);
        }

        [HttpPost]
        public async Task<ActionResult> apiPrice(string dropboxvalue)
        {

            ViewBag.Conditions = new SelectList(db.Conditions, "ID", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "ID", "Grading");

            HttpResponseMessage resp = await yugiohPriceApi.GetAsync($"get_card_prices/{dropboxvalue}");
            if (resp.IsSuccessStatusCode)
            {
                var rsp = await resp.Content.ReadAsStringAsync();

                var li = JObject.Parse(rsp).ToObject<RootObject>();

                SaleConditionGradeVM salevm = new SaleConditionGradeVM()
                {
                    MyCard = dropboxvalue,
                    MyDatum = new List<Datum>(),
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

                return View("Create", salevm);
            }
            SaleConditionGradeVM salev = new SaleConditionGradeVM();
            return View("Create", salev);
        }

        public ActionResult Details(int? id)
        {
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

        public async Task<ActionResult> Create()
        {

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

            HttpResponseMessage response = await yugiohApi.GetAsync($"v4/cardinfo.php?");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();

                salevm.MyCards = li;
                return View(salevm);
            }
            return View(salevm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Price,ForAuction")] SaleConditionGradeVM sale, string dropboxvalue, string Conditions, string Grades)
        {
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
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sale);
        }

        [HttpPost]
        public ActionResult Search(string search)
        {
            List<Sale> li = new List<Sale>();

            foreach (var i in db.Sales.Include("Card.Cardtype").Include("CardGrade").Include("CardCondition").Include("Seller.UserLevel").Where(x => x.Card.name.Contains(search) | x.Card.print_tag.Contains(search) | x.Card.Cardtype.Name == search | x.Seller.Username == search | x.CardGrade.Grading == search))
            {
                li.Add(i);
            }

            return View("Index", li);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editListing([Bind(Include = "ID,Price,ForAuction")] Sale sale, string Conditions, string Grades)
        {
            ViewBag.Conditions = new SelectList(db.Conditions, "ID", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "ID", "Grading");

            Sale sale1 = db.Sales.Include("Card").Include("Seller").Include("CardCondition").Include("CardGrade").Where(x => x.ID == sale.ID).AsNoTracking().FirstOrDefault();

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

            sale1.Price = sale.Price;
            sale1.ForAuction = sale.ForAuction;
            db.Entry(sale1).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult editListing(int? id)
        {
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
            Sale sale = db.Sales.Include("Card").Include("Seller.Collection").Where(x => x.ID == id).FirstOrDefault();
            User user = db.Users.Find(sale.Seller.ID);

            //adds your unsold card to your collection
            user.Collection.Add(sale.Card);
            db.Sales.Remove(sale);
            db.SaveChanges();
            return RedirectToAction("Index");
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
