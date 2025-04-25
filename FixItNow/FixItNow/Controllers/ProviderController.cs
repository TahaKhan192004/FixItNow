using Microsoft.AspNetCore.Mvc;
using FixItNow.Models;
using FixItNow.Models.ViewModel;
using FixItNow.Data;

namespace FixItNow.Controllers
{
    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext c;
        public ProviderController(ApplicationDbContext d)
        {
            c = d;
        }
        public IActionResult Index(int id)
        {
           ProviderRepository repository = new ProviderRepository(c);
           ProviderProfile pp = new ProviderProfile();
           
                var _provider=c.Providers.First(p=> p.Id == id);
                pp.provider = _provider;
            
            pp.projects = repository.getTotalProjects(id);
            pp.clients = repository.getTotalClients(id);
            pp.positiveReviews=repository.getPositiveReviews(id);
            pp.clientSatisfaction = repository.getSatisfactionRate(id);
            return View(pp);
        }
        public IActionResult ProviderPanel()
        {
            var f = HttpContext.Request.Cookies["currentUser"].Split(',');
            int id = Convert.ToInt32(f[0]);
            Console.WriteLine(id);
            ProviderPanel pp=new ProviderPanel();
            ProviderRepository repository = new ProviderRepository(c);
            var bookings = c.Bookings.Where(b => b.providerId == id).ToList();
            var services = c.Services.Where(s => s.providerID == id).ToList();
            var messages = c.Messagess.Where(m => m.providerId == id).ToList();
            List<string> titles = new List<string>();

            if (bookings.Count() > 0 && services.Count() > 0)
            {
                foreach (var y in bookings)
                {
                    var x = c.Services.First(s => s.id == y.serviceId).title;
                    titles.Add(x);
                }

                pp.countOfBookings = services.Select(s =>
                bookings.AsEnumerable().Count(b => b.serviceId == s.id)
                ).ToList();

                pp.revenueOfServices = services.Select(s =>
                bookings.AsEnumerable()
                .Where(b => b.serviceId == s.id)
                .Sum(b => Convert.ToInt32(b.pricing))
                 ).ToList();
            }
                
               
        
               pp.services = services;
               pp.titles = titles;
               pp.messages = messages;
               pp.bookings = bookings;

            
            return View(pp);
        }
    }
}
