using projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt.Areas.Admin.Controllers
{
    public class MenaxhoPorosiController : Controller
    {
        // GET: Admin/MenaxhoPorosi
        projektEntities db = new projektEntities();
        public ActionResult Index(string sort = "emerprodukti", string icon = "fa-sort-asc", int pg = 1)
        {

            var order = db.Orders.ToList();

            ViewBag.sort = sort;
            ViewBag.icon = icon;
            if (ViewBag.sort == "emerprodukti")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    order = order.OrderBy(temp => temp.emerprodukti).ToList();
                else
                    order = order.OrderByDescending(temp => temp.emerprodukti).ToList();
            }

            if (ViewBag.sort == "sasia")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    order = order.OrderBy(temp => temp.sasia).ToList();
                else
                    order = order.OrderByDescending(temp => temp.sasia).ToList();
            }
            if (ViewBag.sort == "status")
            {
                if (ViewBag.icon == "fa-sort-asc")
                    order = order.OrderBy(temp => temp.status).ToList();
                else
                    order = order.OrderByDescending(temp => temp.emerprodukti).ToList();
            }

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(order.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (pg - 1) * NoOfRecordsPerPage;
            ViewBag.Pgnr = pg;
            ViewBag.NoOfPages = NoOfPages;
            order = order.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();
            ViewBag.perdorues = db.AspNetUsers.ToList();
            return View(order);

        }
        public ActionResult ndrysho(int id)
        {
            var order = db.Orders.Where(tmp => tmp.id == id).FirstOrDefault();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //per te rritur sigurine
        public ActionResult ndrysho(Order ord)
        {
            var order = db.Orders.Where(tmp => tmp.id == ord.id).FirstOrDefault();
            var produkt = db.Books1.Where(itm => itm.productid == ord.idproduct).FirstOrDefault();

            //kontroll per sasine
            var sasiedisponueshme = produkt.sasidisponueshme;
            if (ord.sasia > sasiedisponueshme){
                TempData["orderadmin"] = "<script>alert('Sasia qe zgjodhet eshte me e madhe se ajo qe disponojme!');</script>";
                return RedirectToAction("ndrysho", "MenaxhoPorosi",new { area = "Admin",id=ord.id });

            }

            //kontrollojme statusin
            if (ord.status == 1)   
            {
                var orddetail = db.OrderDetails.Where(tmp => tmp.idorder == ord.id).FirstOrDefault();
                if (orddetail != null) //fshijme produktin nga orderdetail ne rast se statusi behet 1 dhe ndryshojme sasine ne stock te produktit
                {
                    produkt.sasidisponueshme = produkt.sasidisponueshme + orddetail.sasiperprodukt;
                    db.OrderDetails.Remove(orddetail);

                    db.SaveChanges();
                }

            } else if( ord.status == 0)
            {
                var orddetail = db.OrderDetails.Where(tmp => tmp.idorder == ord.id).FirstOrDefault();
                var cmimi = produkt.price;
                if (orddetail == null)  //shtojme produktin tek orderdetail ne rast se statusi behet 0
                {

                   
                    OrderDetail orde = new OrderDetail();
                    orde.idorder = ord.id;
                    orde.sasiperprodukt = ord.sasia;
                    orde.data = DateTime.Now;
                    orde.cmimtotalxprodukt = cmimi * ord.sasia;
                    produkt.sasidisponueshme = produkt.sasidisponueshme - ord.sasia;
                    db.OrderDetails.Add(orde);
                    db.SaveChanges();
                }
                else    //reflektojme ndryshimet tek tabela orderdetail
                {
                    orddetail.sasiperprodukt = ord.sasia;
                    orddetail.data = DateTime.Now;
                    orddetail.cmimtotalxprodukt = ord.sasia * cmimi;
                    var difsasi = order.sasia-ord.sasia;  //kontrollojme nese ka ndryshuar sasia 
                    if(difsasi <= 0)
                    {
                        produkt.sasidisponueshme = produkt.sasidisponueshme + difsasi;   //kur vendosim nje sasi me te madhe se ajo qe kishim

                    } else { produkt.sasidisponueshme = produkt.sasidisponueshme + difsasi; } //kur vendosim nje sasi me te vogel se ajo qe kishim
                    
                    db.SaveChanges();
                }
            }

          
            order.idperdorues = ord.idperdorues;
            order.idproduct = ord.idproduct;
            order.sasia = ord.sasia;
            order.status = ord.status;
            order.emerprodukti = ord.emerprodukti;
            db.SaveChanges();

            return RedirectToAction("Index", "MenaxhoPorosi", new { area = "Admin" });
        }


        public ActionResult fshi(int id)
        {
            var order = db.Orders.Where(tmp => tmp.id == id).FirstOrDefault();
            var orderdetail = db.OrderDetails.Where(tmp => tmp.idorder == id).FirstOrDefault();

            if (orderdetail != null)
            {
                db.OrderDetails.Remove(orderdetail);
                db.SaveChanges();
                   
            }

            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index", "MenaxhoPorosi", new { area = "Admin" });
        }
    }
}
