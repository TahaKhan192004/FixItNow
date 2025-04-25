using Microsoft.AspNetCore.Mvc;
using FixItNow.Models;
using FixItNow.Data;

namespace FixItNow.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext c;
        public BookingController(ApplicationDbContext d)
        {
            c = d;
        }
      
        [HttpGet]
        public IActionResult Index(int id)
        {
            TempData["ServiceId"] = id;
            ViewBag.x = id;
            return View();
        }
        [HttpPost]
        public IActionResult Index(string taskDetails,string address,string pricing,DateTime serviceDate,DateTime serviceTime)
        {
            Booking booking = new Booking();
            int id = Convert.ToInt32(TempData["ServiceId"]);
            Console.WriteLine(id);
           
                var service = c.Services.First(s => s.id == id);

                booking.providerId = service.providerID;
                booking.serviceId = service.id;
              

                booking.description = taskDetails;
              

                booking.address = address;
              

                booking.pricing = pricing;
                booking.Time = serviceTime;
              

                booking.Date = serviceDate;
                if(booking.Date<DateTime.Now.Date)
                {
                    ViewBag.DateError = "Select a future date";
                    return View("Index");
                }
               

                var y = HttpContext.Request.Cookies["currentUser"].Split(',');
                booking.customerId = Convert.ToInt32(y[0]);
                if (booking.customerId ==service.providerID)
                {
                    ViewBag.Error = "You cannot avail this service";
                    return View("Index");
                }
              

                c.Bookings.Add(booking);
                c.SaveChanges();
            
            ViewBag.Confirmation = "Booking Confirmed ";
            return View();
        }
      
            

    }
}
