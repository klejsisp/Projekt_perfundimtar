using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projekt.Models;


namespace projekt.Controllers
{
    public class FaturaController : Controller
    {
        // GET: Fatura


        public ActionResult Shikofatur()
        {
            projektEntities dbo = new projektEntities();
            ViewBag.Order = dbo.Orders.ToList();
            var list = (List<OrderDetail>)Session["order"];
            return View(list);
        }
       
    }
}