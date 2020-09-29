using Microsoft.Ajax.Utilities;
using projekt.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt.Controllers
{
    public class BookController : Controller
    {
        projektEntities db = new projektEntities();
        // GET: Book


        //listolibrat me te dhenat e tyre
        public ActionResult Listmelibra(int? idkat)
        {
            
            if (idkat != null)
            {
                
               List<Books> book = db.Books1.Where(tmp =>tmp.categoryid == idkat).ToList();
                var book1 = db.Books1.Where(tmp => tmp.categoryid == idkat).FirstOrDefault().Category.categoryname;
                ViewBag.kategoria = book1;
                return View(book);
            }
            else {
                var librat = db.Books1.OrderByDescending(tmp=>tmp.productid).ToList();
                
                return View(librat);
            }
           
           

        }
       
        //detajet e nje libri sipas nje id-je dhe komentet
        public ActionResult Detaje(int id )
        {
            ViewBag.id = id;
            var prd = db.Books1.Where(tmp => tmp.productid == id).FirstOrDefault();//mer librin qe ka ate id 
            var autor = prd.autori;
            ViewBag.List = db.Books1.Where(tmp => tmp.autori == autor).ToList();  //lista me libra me te njejtin autor
          ViewBag.koment =db.Comments.Where(tmp => tmp.bookid == id).ToList(); //lista me komente per ate produkt
          
            ViewBag.klienti = db.AspNetUsers.ToList();
            return View(prd);
        }


        [HttpPost]
        public ActionResult Detaje(int bookid,int rating,string articleComment)
        {
            if (Session["id"] != null)
            {

                var cm = new Comment();
                cm.rating = rating;
                cm.bookid = bookid;
                cm.commentdescription = articleComment;
                cm.commentdate = DateTime.Now;
                cm.userid = Convert.ToString(Session["id"]);
                db.Comments.Add(cm);
                db.SaveChanges();

                ViewBag.id = bookid;
                var prd = db.Books1.Where(tmp => tmp.productid == bookid).FirstOrDefault();//mer librin qe ka ate id 
                var autor = prd.autori;
                ViewBag.List = db.Books1.Where(tmp => tmp.autori == autor).ToList();  //lista me libra me te njejtin autor
                ViewBag.koment = db.Comments.Where(tmp => tmp.bookid == bookid).ToList(); //lista me komente per ate produkt

                ViewBag.klienti = db.AspNetUsers.ToList();
                return View(prd);
            } else  return RedirectToAction("Index","Logimi");
        }

        //hap nje file
        public ActionResult getfile(int id)
        {
            var lb = db.Books1.Where(tmp => tmp.productid == id).FirstOrDefault().filepreview;
            if (lb != null)
            {

                var file = db.Books1.Where(tmp => tmp.productid == id).FirstOrDefault();
                return File(file.filepreview, "application/pdf");
            }
            else
            {
                return File("/Filet/bookreview.pdf", "application/pdf"); //nje file default
            }
               
                            

        }


    }
}