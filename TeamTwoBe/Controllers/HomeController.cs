using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamTwoBe.Models;
using TeamTwoBe.ViewModels;

namespace TeamTwoBe.Controllers
{

    public class HomeController : Controller
    {
        private Context db = new Context();

        // GET: Home
        public ActionResult Index(int? id)
        {
            HomeViewModel vm = new HomeViewModel()
            {
                Followers = new List<Models.Sale>(),
                Trending = new List<Models.Sale>(),
                Recommended = new List<Models.Sale>(),
            };
            if(id != null)
            {
                User user = db.Users.Include("Following").Where(x => x.ID == id).FirstOrDefault();
                if(user.Following != null)
                {
                    foreach (var i in user.Following)
                    {
                        foreach (var y in db.Sales.Where(x => x.Seller.ID == i.ID))
                        {
                            vm.Followers.Add(y);
                        }
                    }
                }
                foreach(var i in db.Sales)
                {
                    vm.Trending.Add(i);
                }
                foreach (var i in db.Sales.Include("Seller.UserLevel").Where(x => x.Seller.UserLevel.ID == 3))
                {
                    vm.Recommended.Add(i);
                }
            }

            return View(vm);
        }

        public ActionResult WhyUs()
        {

            return View();

        }
    }
}