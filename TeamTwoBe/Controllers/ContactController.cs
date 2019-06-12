using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TeamTwoBe.Models;
using System.Text;
using System.Threading;
using System.Net;

namespace TeamTwoBe.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Contact()
        {
            return View();
        }
        private Context db = new Context();

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

        [HttpPost]
        public ActionResult Form(string email, string subject, string message)
        {
            checkCookie();
            try
            {
                if (ModelState.IsValid)
                {
                    var senderemail = new MailAddress("contact@2btrading.co.nz", "2B");
                    var receiveremail = new MailAddress(email, "Receiver");
                    var password = "@DXjT3n4JtFGA";
                    var sub = subject;
                    var body = message;

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.2btrading.co.nz",
                        Port = 25,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderemail.Address, password)

                    };
                    using (var mess = new MailMessage(senderemail, receiveremail))
                    {
                        sub = subject;
                        body = message;
                        smtp.Send(mess);
                    }


                };
            }
            catch (Exception)
            {
                ViewBag.Error = "There are some problems in sending email";
            }
            return View("Contact");
        }

    }
}


