using Microsoft.AspNetCore.Mvc;
using FixItNow.Models;
using FixItNow.Data;

namespace FixItNow.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext c;
        public ReviewController(ApplicationDbContext d)
        {
            c = d;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(int rating, int serviceid,string comments)
        {
            Review r = new Review();
            r.comments = comments;
            r.dateTime = DateTime.Now;
            r.rating    = rating;
            r.serviceId = serviceid;
            var y = HttpContext.Request.Cookies["currentUser"].Split(',');
            r.customerId= Convert.ToInt32(y[0]);
           
                var x= c.Userss.First(u=>u.id==r.customerId).firstName;
                r.name=x;
                c.Reviews.Add(r);
                c.SaveChanges();
            

            return RedirectToAction("UserCart", "Services");
        }

    }
}
