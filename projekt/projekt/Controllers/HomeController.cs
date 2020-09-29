using projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace projekt.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string search="")
        {
            var db = new projektEntities();
         var books = db.Books1.Where(tmp=>tmp.isbookoftheweek==1).ToList();
            ViewBag.search = search;
             ViewBag.src = books.Where(temp => temp.autori.Contains(search) || temp.name.Contains(search) || temp.Category.categoryname.Contains(search)).ToList();
            
            return View(books);
        }
        
       
        
    }
}