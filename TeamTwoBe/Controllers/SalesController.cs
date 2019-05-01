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

        private HttpClient yugiohApi;


    // GET: Sales
    public ActionResult Index()
        {
            return View(db.Sales.ToList());
        }

        public async Task<ActionResult> getCards(string test)
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("https://db.ygoprodeck.com/api/")
            };

            HttpResponseMessage response = await client.GetAsync($"v4/cardinfo.php?fname={test}");
            if (response.IsSuccessStatusCode)
            {
                var rsp = await response.Content.ReadAsStringAsync();

                rsp = rsp.Substring(1, rsp.Length - 2);
                List<Card> li = JArray.Parse(rsp).ToObject<List<Card>>();


                SaleConditionGradeVM salevm = new SaleConditionGradeVM()
                {
                    user = db.Users.Find(Session["UserID"]),
                    MyCards = li,
                };
                ViewBag.Conditions = new SelectList(db.Conditions, "Id", "CardCondition");
                ViewBag.Grades = new SelectList(db.Grades, "Id", "Grading");

                return View("Create", salevm);
            }
            return View();

        }

        // GET: Sales/Details/5
        public ActionResult Details(int? id)
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
            return View(sale);
        }

        // GET: Sales/Create
        public ActionResult Create(List<Card> li)
        {

            //These help get the data into the view for the dropdown list.
            ViewBag.Conditions = new SelectList(db.Conditions, "Id", "CardCondition");
            ViewBag.Grades = new SelectList(db.Grades, "Id", "Grading");

            //Stops anyone from creating a new sale if they are not logged in as a valid user. ~Joe
            if (Session["userID"] == null)
            {
                return RedirectToAction("login", "users");
            }

            if(li != null)
            {
                SaleConditionGradeVM salevm = new SaleConditionGradeVM()
                {
                    user = db.Users.Find(Session["UserID"]),
                    MyCards = li,
                };
                return View(salevm);
            }
            SaleConditionGradeVM sale = new SaleConditionGradeVM()
            {
                user = db.Users.Find(Session["UserID"]),
                MyCards = li,
            };

            return View(sale);

        }

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Price,ForAuction")] SaleConditionGradeVM sale, string test, string Conditions, string Grades, float Price)
        {
            if (ModelState.IsValid)
            {
                Card card = new Card()
                {
                    name = test,
                    Cardtype = db.CardTypes.Find(1),
                };
                db.Cards.Add(card);
                Sale testsale = new Sale();
                sale.MySale = testsale;
                sale.MySale.Card = card;
                Grade grade = db.Grades.Find(Convert.ToInt32(Grades));
                Condition condition = db.Conditions.Find(Convert.ToInt32(Conditions));
                sale.MySale.CardCondition = condition;
                sale.MySale.CardGrade = grade;
                User user = db.Users.Find(Session["UserID"]);
                sale.MySale.Seller = user;
                sale.MySale.Price = Price;
                sale.MySale.ID = sale.ID;
                sale.MySale.IsSold = false;
                sale.MySale.Buyer = null;
                db.Sales.Add(sale.MySale);
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
