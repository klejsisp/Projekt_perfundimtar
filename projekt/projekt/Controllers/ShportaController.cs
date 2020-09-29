using projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace projekt.Controllers
{
    public class ShportaController : Controller
    {
        projektEntities db = new projektEntities();
        // GET: Shporta
        public ActionResult Index(int id)
        {
   
             if (Session["id"] != null)
              {
                var per = Convert.ToString(Session["id"]);
                var nrord = db.Orders.Where(tmp => tmp.idproduct == id && tmp.idperdorues == per && tmp.status==1).Count();
                if (nrord >= 1) //nese ekziston porosia per ate produkt  kontrollojme sasine e tij 
                {

                    var order = db.Orders.Where(tmp => tmp.idproduct == id && tmp.idperdorues == per && tmp.status ==1).FirstOrDefault();
                    var produkt = db.Books1.Where(tmp =>tmp.productid ==id).FirstOrDefault();
                    var sasiaeinkremetuar = order.sasia + 1;
                    var sasidis = produkt.sasidisponueshme;
                    if (sasiaeinkremetuar > sasidis)       //kontrollojme nese nuk e kalon sasine e disponueshme qe kemi ne stock per ate produkt
                    {
                        TempData["order"] = "<script>alert('Momentalisht nuk disponojme nga ky produkt !');</script>";
                        return RedirectToAction("Index", "Home");

                    }
                    else   //e inkrementojme
                    {

                        var sasiprf = order.sasia + 1;
                        order.sasia = sasiprf;
                        db.SaveChanges();
                        var d = Convert.ToString(Session["id"]);
                        List<Order> lista2 = db.Orders.Where(tmp => tmp.idperdorues == per && tmp.status == 1).ToList();
                        return View(lista2);

                    }


                }
                else  //nese ky klient po porosit per here te pare kete produkt
                {
                    var book = db.Books1.Where(tmp => tmp.productid == id).FirstOrDefault();
                    Order orde = new Order();
                    orde.idproduct = (int)id;
                    orde.emerprodukti = book.name;
                    orde.idperdorues = Convert.ToString(Session["id"]);
                    orde.sasia = 1;
                    orde.status = 1;
                    db.Orders.Add(orde);
                    db.SaveChanges();
                   var lista1 = db.Orders.Where(tmp => tmp.idperdorues == per && tmp.status == 1).ToList();
                    return View(lista1);

                }
            } //nese sje i loguar te con tek faqja e logimit
            else return RedirectToAction("Index", "Logimi"); 
                
             }

        public ActionResult Shikoshporten()
        {
            var a = Convert.ToString(Session["id"]);
            var lista = db.Orders.Where(tmp => tmp.idperdorues == a && tmp.status == 1).ToList();
            return View("Index",lista);
        }


        public ActionResult Shto(int id)
        {
            var prs = Convert.ToString(Session["id"]);
            var sasid = db.Books1.Where(tmp => tmp.productid == id).FirstOrDefault();
            var sasidis = sasid.sasidisponueshme;//sasia ne stock qe kemi per ate produkt
            var order = db.Orders.Where(tmp => tmp.idproduct == id && tmp.idperdorues==prs && tmp.status == 1).FirstOrDefault(); //nr i orders qe kemi per ate produkt
            var or = order.sasia + 1;
            if(or > sasidis)
            {
                TempData["orderi"] = "<script>alert('Sasia qe zgjodhet eshte me e madhe se ajo qe disponojme ne stock !');</script>";
                     
            }
            else {
                
                var sasi = order.sasia + 1;
                order.sasia = sasi;
                db.SaveChanges();
            }
           
            var ord1 = db.Orders.Where(tmp =>tmp.idperdorues == prs && tmp.status == 1).ToList();
            return View("Index",ord1);
        }
        public ActionResult Hiq(int id)
        {
            var ord = Convert.ToString(Session["id"]);
            var order = db.Orders.Where(tmp => tmp.idproduct == id && tmp.idperdorues==ord && tmp.status == 1).FirstOrDefault().sasia; //nr sasise orders qe kemi per ate produkt
            var nr= order-1;
            
            if (nr <= 0)
            {
                var order1 = db.Orders.Where(tmp => tmp.idproduct == id && tmp.idperdorues == ord && tmp.status == 1).FirstOrDefault();
                db.Orders.Remove(order1);
                db.SaveChanges();
            }
            else
            {if (nr > 0)
                {
                    var orderi = db.Orders.Where(tmp => tmp.idproduct == id && tmp.idperdorues == ord && tmp.status == 1).FirstOrDefault();
                    var sas= orderi.sasia - 1;
                    orderi.sasia = sas;
                    db.SaveChanges();
                  var ordi = db.Orders.Where(tmp => tmp.idperdorues == ord && tmp.status == 1).ToList();
                    return View("Index", ordi);
                }

            }
           
            var ord1 = db.Orders.Where(tmp => tmp.idperdorues == ord && tmp.status == 1).ToList();
            return View("Index", ord1);
        }


        public ActionResult Fshi(int id)
        {
            var ord = Convert.ToString(Session["id"]);
            var order = db.Orders.Where(tmp => tmp.idproduct == id && tmp.idperdorues == ord && tmp.status == 1).FirstOrDefault();
            db.Orders.Remove(order);
            db.SaveChanges();
         
            var ord1 = db.Orders.Where(tmp => tmp.idperdorues == ord && tmp.status == 1).ToList();
            return View("Index", ord1);
        }
    }
}