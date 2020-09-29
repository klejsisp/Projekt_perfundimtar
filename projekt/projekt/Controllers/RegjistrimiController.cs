using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.IO;
using projekt.Models;
using System.Security.Cryptography;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using projekt.Identity;
using System.Web.Helpers;
using projekt.ViewModels;

namespace projekt.Controllers
{
    public class RegjistrimiController : Controller
    {
        
        // GET: Regjistrimi
        public ActionResult Index()
        {
            
            return View();
           
        }
        [HttpPost]
        public ActionResult Index(RegjistrimViewModel kl)
        {

            if (ModelState.IsValid)
            {
                //register
                var appDbContext = new ApplicationDbContext();
                var userStore = new ApplicationUserStore(appDbContext);
                var userManager = new ApplicationUserManager(userStore);
                var passwordHash = Crypto.HashPassword(kl.password);
                var photo = "";
                if (Request.Files.Count >= 1)
                {
                    var file = Request.Files[0];
                    var imgBytes = new Byte[file.ContentLength];
                    file.InputStream.Read(imgBytes, 0, file.ContentLength);
                    var base64String = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);
                    photo = base64String;
                } 

                var user = new ApplicationUser() { Email = kl.email, UserName = kl.username, PasswordHash = passwordHash,emri=kl.emri,gjinia=kl.gjinia,imgurl=photo };
                IdentityResult result = userManager.Create(user);

                if (result.Succeeded)
                {
                    //role
                    userManager.AddToRole(user.Id, "Customer");

                    //login
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new AuthenticationProperties(), userIdentity);
                }

                TempData["regjistrim"]= "<script>alert('Ju u regjistruat me sukses !');</script>";
                return View();
            }
            else
            {
                ModelState.AddModelError("My Error", "Invalid data");
                return View();
            }

        }
    }
}