using projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace projekt.Controllers
{
    public class ProfiliController : Controller
    {
        // GET: Profili
        projektEntities db = new projektEntities();
        public ActionResult Index()
        {
            
             var kl=Convert.ToString(Session["id"]);
            var user = db.AspNetUsers.Where(tmp => tmp.Id == kl).FirstOrDefault();
            return View(user);
        }

        public ActionResult update()
        {
            var kl = Convert.ToString(Session["id"]);

            var user = db.AspNetUsers.Where(tmp => tmp.Id == kl).FirstOrDefault();
            return View(user);


        }

        [HttpPost]
        public ActionResult update(AspNetUser kl)
        { if (ModelState.IsValid)
            {
                var passwordHash = Crypto.HashPassword(kl.PasswordHash);
                var photo = "";
                if (Request.Files.Count >= 1)
                {
                    var file = Request.Files[0];
                    var imgBytes = new Byte[file.ContentLength];
                    file.InputStream.Read(imgBytes, 0, file.ContentLength);
                    var base64String = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);
                    photo = base64String;
                }
                var cs = Convert.ToString(Session["id"]);
                var user = db.AspNetUsers.Where(tmp => tmp.Id == cs).FirstOrDefault();
                user.PasswordHash = passwordHash;
                user.imgurl = photo;
                user.gjinia = kl.gjinia;
                user.emri = kl.emri;
                user.UserName = kl.UserName;
                user.Email = kl.Email;
                db.SaveChanges();
                return RedirectToAction("Index", "Profili");
            }
            else return View();
        }

        public ActionResult delete()
        {
            var kl = Convert.ToString(Session["id"]);
            
            var klient = db.AspNetUsers.Where(tmp => tmp.Id == kl).FirstOrDefault();
            var order = db.Orders.Where(tmp => tmp.idperdorues == kl).ToList();
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
            var koment = db.Comments.Where(tmp => tmp.userid == kl).ToList();
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
            Session["id"] = null;
            return RedirectToAction("Index","Home");
        }
    }
}