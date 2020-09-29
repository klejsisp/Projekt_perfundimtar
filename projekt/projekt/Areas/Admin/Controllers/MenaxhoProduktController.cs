using Microsoft.AspNet.Identity;
using projekt.Models;
using projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt.Areas.Admin.Controllers
{
    public class MenaxhoProduktController : Controller
    {
        // GET: Admin/MenaxhoProdukt
        projektEntities db = new projektEntities();
        public ActionResult Index(string sort = "name", string icon = "fa-sort-asc", int pg = 1)
        {
            
            var book = db.Books1.ToList();

            ViewBag.sort = sort;
            ViewBag.icon = icon;
            if (ViewBag.sort == "name")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    book = book.OrderBy(temp => temp.name).ToList();
                else
                    book = book.OrderByDescending(temp => temp.name).ToList();
            }

            if (ViewBag.sort == "autori")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    book = book.OrderBy(temp => temp.autori).ToList();
                else
                    book = book.OrderByDescending(temp => temp.autori).ToList();
            }
            if (ViewBag.sort == "price")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    book = book.OrderBy(temp => temp.price).ToList();
                else
                    book = book.OrderByDescending(temp => temp.price).ToList();
            }
            if (ViewBag.sort == "sasidisponueshme")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    book = book.OrderBy(temp => temp.sasidisponueshme).ToList();
                else
                    book = book.OrderByDescending(temp => temp.sasidisponueshme).ToList();
            }

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(book.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (pg - 1) * NoOfRecordsPerPage;
            ViewBag.Pgnr = pg;
            ViewBag.NoOfPages = NoOfPages;
            book = book.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            ViewBag.Comment = db.Comments.ToList();
            return View(book);
            
        }


  

        public ActionResult shto()
        {
            ViewBag.kategori = db.Categories.ToList();
            return View();
        

        }

        [HttpPost]
        public ActionResult shto(Books bk)
        {
            if (ModelState.IsValid)
            {
                var photo = "";
                if (Request.Files.Count >= 1)
                {
                    var file = Request.Files[0];
                    var imgBytes = new Byte[file.ContentLength];
                    file.InputStream.Read(imgBytes, 0, file.ContentLength);
                    var base64String = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);
                    photo = base64String;
                }
                bk.imageurl = photo;
                bk.categoryid = Convert.ToInt32(bk.categoryid);
                db.Books1.Add(bk);
                db.SaveChanges();
                return RedirectToAction("Index", "MenaxhoProdukt", new { area = "Admin" });
            } else return RedirectToAction("Index", "MenaxhoProdukt", new { area = "Admin" });
        }

        public ActionResult ndrysho(int id)
        {
            var book = db.Books1.Where(tmp => tmp.productid == id).FirstOrDefault();
            ViewBag.kategori = db.Categories.ToList();
            return View(book);
        }
       
        [HttpPost]
        public ActionResult ndrysho(Books bk)
        {
            var photo = "";
            if (Request.Files.Count >= 1)
            {
                var file = Request.Files[0];
                var imgBytes = new Byte[file.ContentLength];
                file.InputStream.Read(imgBytes, 0, file.ContentLength);
                var base64String = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);
                photo = base64String;
               
            }
            var books = db.Books1.Where(tmp=>tmp.productid==bk.productid).FirstOrDefault();
            books.imageurl = photo;
            books.name = bk.name;
            books.isbookoftheweek = bk.isbookoftheweek;
            books.longdescription = bk.longdescription;
            books.autori = bk.autori;
            books.price = bk.price;
            books.categoryid = Convert.ToInt32(bk.categoryid);
            books.sasidisponueshme = bk.sasidisponueshme;
            db.SaveChanges();
            return RedirectToAction("Index", "MenaxhoProdukt", new { area = "Admin" });
        }

        public ActionResult fshi(int id)
        {
            var produkt = db.Books1.Where(tmp => tmp.productid == id).FirstOrDefault();
            var order = db.Orders.Where(tmp => tmp.idproduct == id).ToList();
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
            var koment = db.Comments.Where(tmp => tmp.bookid == id).ToList();
            if (koment != null)
            {
                foreach (var cm in koment)
                {
                    var comment = db.Comments.Where(tmp => tmp.idkoment == cm.idkoment).FirstOrDefault();
                    db.Comments.Remove(comment);
                }
            }
            db.Books1.Remove(produkt);
            db.SaveChanges();
            return RedirectToAction("Index", "MenaxhoProdukt", new { area = "Admin" });
        }
    
    }
}