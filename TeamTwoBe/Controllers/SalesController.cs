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
            Session["View"] = "SaleIndex";
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
            Session["View"] = "SaleDetail";
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

            Session["View"] = "SaleCreate";
            if (li != null)
            {
                SaleConditionGradeVM salevm = new SaleConditionGradeVM()
                {
                    MyCards = li,
                };
                return View(salevm);
            }
            SaleConditionGradeVM sale = new SaleConditionGradeVM()
            {
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
