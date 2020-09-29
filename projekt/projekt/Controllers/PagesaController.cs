using PayPal.Api;
using projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projekt.Controllers
{
    public class PagesaController : Controller
    {
        // GET: Pagesa

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext
            APIContext apiContext = PayPalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal
                //Payer Id will be returned when payment proceeds or click to pay
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class
                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Pagesa/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session
                    //which will be used in the payment execution
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                PayPalLogger.Log("Error: " + ex.Message);
                return View("FailureView");
            }
            //on successful payment, show success page to user.
            //update status
            String kl = Convert.ToString(Session["id"]);
            projektEntities db = new projektEntities();
            var porosi = db.Orders.Where(tmp => tmp.idperdorues == kl).ToList();
            foreach(var item in porosi) { item.status = 0; }
            db.SaveChanges();
            //insertin orderdetail
            projektEntities dbor = new projektEntities();
            var orderdetail = new projekt.Models.OrderDetail();
            var list = (List<projekt.Models.OrderDetail>)Session["order"];
            
            foreach(var item in list)
            {
                orderdetail.idorder = item.idorder;
                orderdetail.sasiperprodukt = item.sasiperprodukt;
                orderdetail.cmimtotalxprodukt = item.cmimtotalxprodukt;
                orderdetail.data = DateTime.Now;

            }
            dbor.OrderDetails.Add(orderdetail);
            dbor.SaveChanges();

            //redukto sasise e disponueshme tek produkti
            projektEntities dbo = new projektEntities();
            var produkt = dbo.Books1.ToList();
            foreach (var or in list)
            {
                foreach (var itm in produkt)
                {
                    if (itm.productid == or.idorderdetail) //ne idorderdetail te session kemi ruajtur id-ne e produktit qe kemi tek tabela Order,kurse orderdetail_id ne nuk e ruajme ne session pasi e kemi bere qe te inkrementohet me nje ne db
                    {
                        itm.sasidisponueshme = itm.sasidisponueshme - or.sasiperprodukt;
                    }
                }
            }
            dbo.SaveChanges();
           
            TempData["pagesa"] = "<script>alert('Pagesa u krye me sukses !');</script>";
            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

          String cmimi = Convert.ToString(Session["price"]);
          

                //Adding Item Details like name, currency, price etc

                itemList.items.Add(new Item()
            {
                name = "E-Book Order",
                currency = "USD",
                price = cmimi,
                quantity = "1",
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details
            var details = new Details()
            {
                tax = "0.1",
                shipping = "1",
                subtotal = "1"
            };
            //Final amount with details
            var amount = new Amount()
            {
                currency = "USD",
                total = "2.1", // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction
            transactionList.Add(new Transaction()
            {
                description = "E-Book Order Transaction",
                invoice_number = Convert.ToString((new Random()).Next(100000)), //invoice number duhet te jete i ndryshem pas cdo pagese
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }





    }
}