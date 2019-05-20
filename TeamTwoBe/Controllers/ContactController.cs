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



        [HttpPost]
        public ActionResult Form(string email, string subject, string message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderemail = new MailAddress("teamb7692@gmail.com", "2B");
                    var receiveremail = new MailAddress(email, "Receiver");
                    var password = "TeamTwoB";
                    var sub = subject;
                    var body = message;

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
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


