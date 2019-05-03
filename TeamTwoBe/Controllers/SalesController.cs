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
using TeamTwoBe.Views.ViewModels;

namespace TeamTwoBe.Controllers
{
    public class SalesController : Controller
    {
        private Context db = new Context();

        private HttpClient yugiohApi = new HttpClient()
        {
            BaseAddress = new Uri("https://db.ygoprodeck.com/api/")
        };
        private HttpClient yugiohPriceApi = new HttpClient()
        {
            BaseAddress = new Uri("http://yugiohprices.com/api/")
        };

        // GET: Sales
        public ActionResult Index()
        {
            Session["View"] = "SaleIndex";
            List<Sale> li = new List<Sale>();
            foreach(var i in db.Sales.Include("Card").Include("Seller.UserLevel"))
            {
                li.Add(i);
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
                    MyDatum = li.data,
                    MyCard = dropboxvalue,
                };

                if(db.Cards.Where(x=> x.name == dropboxvalue).FirstOrDefault() == null)
                {
                    foreach (var i in li.data)
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
                        db.Cards.Add(card);
                    }
                    db.SaveChanges();
                }
                
                return View("Create", salevm);
            }
            SaleConditionGradeVM salev = new SaleConditionGradeVM();
            return View("Create", salev);
        }

        // GET: Sales/Details/5
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

        // GET: Sales/Create
        public async Task<ActionResult> Create()
        {

            ViewBag.Conditions = new SelectList(db.Conditions, "ID", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "ID", "Grading");

            //Stops anyone from creating a new sale if they are not logged in as a valid user. ~Joe
            if (Session["userID"] == null)
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

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Price,ForAuction")] SaleConditionGradeVM sale, string dropboxvalue, string Conditions, string Grades, float Price)
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
                    Price = Price,
                    IsSold = false,
                };

                db.Sales.Add(MySale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sale);
        }

        // GET: Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            Session["View"] = "SaleEdit";
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Price,ForAuction")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sale);
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            Session["View"] = "SaleDelete";
            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sale sale = db.Sales.Find(id);
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
