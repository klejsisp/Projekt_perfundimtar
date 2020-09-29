using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using projekt.Identity;
using projekt.Models;
using projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace projekt.Controllers
{
    public class LogimiController : Controller
    {
        // GET: Logimi
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogimViewModel k)
        {
            if (ModelState.IsValid)
            {
                //login
                var appDbContext = new ApplicationDbContext();
                var userStore = new ApplicationUserStore(appDbContext);
                var userManager = new ApplicationUserManager(userStore);
                var user = userManager.Find(k.Username, k.Password);
                if (user != null)
                {
                    //login
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new AuthenticationProperties(), userIdentity);
                    Session["id"] = user.Id;
                    if (userManager.IsInRole(user.Id, "Admin"))
                    {
                        return RedirectToAction("Index", "Faqjakryesore", new { area = "Admin" });
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Mesazh = "Username ose Password i pasakte !";
                    ModelState.AddModelError("myerror", "Invalid username or password");
                    return View();
                }
            }
            else return View();

        }
    }
}