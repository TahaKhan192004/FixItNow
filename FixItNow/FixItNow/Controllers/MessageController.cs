using Microsoft.AspNetCore.Mvc;
using FixItNow.Data.Migrations;
using FixItNow.Models;
using FixItNow.Data;

namespace FixItNow.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext c;
        public MessageController(ApplicationDbContext d)
        {
            c = d;
        }
        [HttpGet]
        public IActionResult Index(int id)
        {
            TempData["ServiceId"] = id;
            return View();
        }
        [HttpPost]
        public IActionResult Index(string from,string messagetitle,string message)
        {
            int id = Convert.ToInt32(TempData["ServiceId"]);
            
                var x = c.Services.First(s => s.id == id);
                Messages m=new Messages();
                m.title = messagetitle;
                m.message=message;
                m.senderName = from;
                m.serviceName = x.title;
                m.DateTime = DateTime.Now.ToString();
                m.serviceId = x.id;
                m.providerId = x.providerID;
                var y = HttpContext.Request.Cookies["currentUser"].Split(',');
                m.customerId = Convert.ToInt32(y[0]);
                if(m.customerId==m.providerId)
                {
                    ViewBag.Error = "Cannot Send message to yourself";
                    return View();
                }
                c.Messagess.Add(m);
                c.SaveChanges();
            
            ViewBag.Sent = "Message Sent";
            return View();
        }
        public IActionResult Inbox()
        {
            var y = HttpContext.Request.Cookies["currentUser"].Split(',');
            int userId= Convert.ToInt32(y[0]);
            if (y[1]=="user")
            {
               
                    var list = c.Messagess.Where(m => m.customerId == userId).ToList();
                    return View(list);
                
            }
            else
            {
               
                    var list = c.Messagess.Where(m => m.providerId == userId).ToList();
                    return View(list);
                
            }
           
        }
        public IActionResult Read(int id)
        {
           
                var me = c.Messagess.First(m => m.Id == id);
                return View(me);
            

        }

    }
}
