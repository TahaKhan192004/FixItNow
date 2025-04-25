using Microsoft.AspNetCore.Mvc;
using FixItNow.Models;
using FixItNow.Models.Repository;
using FixItNow.Models.ViewModel;
using FixItNow.Data;

namespace FixItNow.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext c;
        public AdminController(ApplicationDbContext d)
        {
            c = d;
        }

        public IActionResult Index()
        {
            AdminRepository _ar=new AdminRepository(c);
            Admin admin = new Admin();
            admin.users = _ar.getListOfUsers();
            admin.providers = _ar.getListOfProviders();
            admin.reviews = _ar.getListOfReviews(); 
            admin.bookings=_ar.getListOfBooking();
            decimal rev = 0;
            foreach(var item in admin.bookings)
            {
                rev = rev + Convert.ToDecimal(item.pricing);
            }
            admin.revenue= rev;

            return View(admin);
        }
        public IActionResult Approve(int id)
        {
            var x = c.Providers.First(p => p.Id == id);
            x.status = "approved";
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Reject(int id)
        {

            var x = c.Providers.First(p => p.Id == id);
            x.status = "rejected";
            c.SaveChanges();
            return RedirectToAction("Index");

        }
        public IActionResult AllUsers()
        {
            AdminRepository _ar = new AdminRepository(c);
            List<User> ul = _ar.getListOfUsers();
            return View(ul);
        }
        public IActionResult AllBookings()
        {
            AdminRepository _ar = new AdminRepository(c);
            List<Booking> ul = _ar.getListOfBooking();
            return View(ul);
        }


    }
}
