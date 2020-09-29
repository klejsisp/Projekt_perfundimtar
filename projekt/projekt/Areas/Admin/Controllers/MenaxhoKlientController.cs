using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projekt.Models;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using projekt.ViewModels;
using projekt.Identity;
using Microsoft.Owin.Security;

namespace projekt.Areas.Admin.Controllers
{
    public class MenaxhoKlientController : Controller
    {
        // GET: Admin/MenaxhoKlient
        projektEntities db = new projektEntities();
        public ActionResult Index(string sort= "emri", string icon= "fa-sort-asc",int pg=1)
        {
            string adm = User.Identity.GetUserId();
            var klient = db.AspNetUsers.Where(tmp => tmp.Id != adm).ToList();
         
            ViewBag.sort = sort;
            ViewBag.icon = icon;
            if (ViewBag.sort == "UserName")
                {
                    if (ViewBag.icon == "fa-sort-asc")
                       klient  = klient.OrderBy(temp => temp.UserName).ToList();
                    else
                       klient = klient.OrderByDescending(temp => temp.UserName).ToList();
                }

            if (ViewBag.sort == "emri")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    klient = klient.OrderBy(temp => temp.emri).ToList();
                else
                    klient = klient.OrderByDescending(temp => temp.emri).ToList();
            }


            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(klient.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (pg - 1) * NoOfRecordsPerPage;
            ViewBag.Pgnr = pg;
            ViewBag.NoOfPages = NoOfPages;
            klient = klient.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(klient);
        }
        public ActionResult shto()
        {
            return View();

        }
        [HttpPost]
        public ActionResult shto(RegjistrimViewModel kl)
        {
            if (ModelState.IsValid)
            {
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

                var user = new ApplicationUser() { Email = kl.email, UserName = kl.username, PasswordHash = passwordHash, emri = kl.emri, gjinia = kl.gjinia, imgurl = photo };
                IdentityResult result = userManager.Create(user);

                if (result.Succeeded)
                {
                    //role
                    userManager.AddToRole(user.Id, "Customer");

                    
                }

                return RedirectToAction("Index", "MenaxhoKlient", new { area = "Admin" });
            }
            else return View();


        }
        public ActionResult ndrysho(string id)
        {
            var klient = db.AspNetUsers.Where(tmp=>tmp.Id==id).FirstOrDefault();
            return View(klient);
        }
        [HttpPost]
        public ActionResult ndrysho(AspNetUser kl)
        {
            if (ModelState.IsValid)
            {
                var klient = db.AspNetUsers.Where(tmp => tmp.Id == kl.Id).FirstOrDefault();
                var pasuord = Crypto.HashPassword(kl.PasswordHash);
                string url = "";
                if (Request.Files.Count >= 1)
                {
                    var file = Request.Files[0];
                    var imgBytes = new Byte[file.ContentLength];
                    file.InputStream.Read(imgBytes, 0, file.ContentLength);
                    var base64String = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);
                    url = base64String;
                }
                klient.PasswordHash = pasuord;
                klient.imgurl = url;
                klient.UserName = kl.UserName;
                klient.emri = kl.emri;
                klient.Email = kl.Email;
                klient.gjinia = kl.gjinia;
                db.SaveChanges();
                return RedirectToAction("Index", "MenaxhoKlient", new { area = "Admin" });
            }
            else
            {

                TempData["ndryshomsg"] = "<script>alert('Ju lutem vendosni te dhenat e sakta !');</script>";
                return RedirectToAction("ndrysho", "MenaxhoKlient", new { @id = kl.Id });
            }
        }

        public ActionResult fshi(string id)
        {
            var klient = db.AspNetUsers.Where(tmp => tmp.Id == id).FirstOrDefault();
            var order=db.Orders.Where(tmp => tmp.idperdorues == id).ToList();
            if (order != null)
            {
                foreach (var item in order)
                {
                    var ord = db.Orders.Where(tmp => tmp.id == item.id).FirstOrDefault();
                    var orddetail = db.OrderDetails.Where(tmp => tmp.idorder == item.id).FirstOrDefault();
                    if (orddetail != null)
                    {
                        db.OrderDetails.Remove(orddetail);
                    }
                    db.SaveChanges();
                    db.Orders.Remove(ord);
                    db.SaveChanges();
                }
            }
            var koment = db.Comments.Where(tmp => tmp.userid == id).ToList();
            if (koment != null)
            {
                foreach (var cm in koment)
                {
                    var comment = db.Comments.Where(tmp => tmp.idkoment == cm.idkoment).FirstOrDefault();
                    db.Comments.Remove(comment);
                }
            }
            db.AspNetUsers.Remove(klient);
            db.SaveChanges();
            return RedirectToAction("Index", "MenaxhoKlient",new { area="Admin"});
        }
    }
}