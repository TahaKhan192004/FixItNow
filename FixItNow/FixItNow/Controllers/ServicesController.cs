using Microsoft.AspNetCore.Mvc;
using FixItNow.Data.Migrations;
using FixItNow.Models;
using FixItNow.Models.Repository;
using FixItNow.Models.ViewModel;
using FixItNow.Data;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext c;
       

        public ServicesController(IWebHostEnvironment env,ApplicationDbContext d)
        {
            _webHostEnvironment = env;
            c = d;
        }
        public IActionResult MyAction(string inputData)
        {
            Console.WriteLine("called");
            
            return PartialView("_MyPartial", model:inputData);
        }

        public IActionResult Index()
        {
          
                var service = c.Services.ToList();
                return View(service);

            
        }
        [HttpGet]
        public IActionResult AddService()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddService(IFormFile serviceImage, string provider, string title, string description, string category, string features, float price)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(category))
            {
                ViewBag.ErrorMessage = "All fields are required!";
                return View();
            }

            string wwwPath = this._webHostEnvironment.WebRootPath;
            string uploadPath = Path.Combine(wwwPath, "uploads", "UploadedImages"); 
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string imagePath = null;
            if (serviceImage != null)
            {
                var imageFileName = Path.GetFileNameWithoutExtension(serviceImage.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(serviceImage.FileName);


                string relativeImagePath = Path.Combine("uploads", "UploadedImages", imageFileName).Replace("\\", "/");
                string fullImagePath = Path.Combine(uploadPath, imageFileName);
                using (var stream = new FileStream(fullImagePath, FileMode.Create))
                {
                    serviceImage.CopyTo(stream);
                }
                imagePath = relativeImagePath;
            }
            var featureList = features?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var y=HttpContext.Request.Cookies["currentUser"].Split(',');
            var service = new Service
            {
                title = title,
                description = description,
                category = category,
                features = featureList,
                provider = provider,
                pricing = price,
                providerID = Convert.ToInt32(y[0]),
                referenceImagePath = imagePath
            };

          
                c.Services.Add(service);
                c.SaveChanges();
            


            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult ServiceDetails(int id)
        {
            ServicesDetails sr = new ServicesDetails();
           
                var x = c.Services.First(s => s.id == id);
                sr.service = x;
                var revs = c.Reviews.Where(r => r.serviceId == id).ToList();
                sr.Reviews = revs;
                int t = 0, one = 0, two = 0, three = 0, four= 0, five = 0;
                foreach(var reviews in revs)
                {
                    t++;
                    if(reviews.rating==1)
                    {
                        one++;
                    }
                    else if(reviews.rating==2)
                    {
                        two++;
                    }
                    else if(reviews.rating==3)
                    {
                        three++;
                    }
                    else if (reviews.rating == 4)
                    {
                        four++;
                    }
                    else
                    {
                        five++;
                    }

                }
                sr.oneStar = one; sr.twoStar = two; sr.threeStar = three;  sr.fourStar= four; sr.fiveStar= five;
                sr.totalReviews = t;
                var relatedS = c.Services.Where(y => y.category == x.category).ToList();
                sr.RelatedServices = relatedS;

                return View(sr);
            }
        
        public IActionResult Save(int id)
        {
           
                var x = c.Services.First(s => s.id == id);
                SavedServices s= new SavedServices();
                s.serviceId = id;
                s.serviceName = x.title;
                var y = HttpContext.Request.Cookies["currentUser"].Split(',');
                s.customerId = Convert.ToInt32(y[0]);
                c.SavedServicess.Add(s);
                c.SaveChanges();
                return RedirectToAction("ServiceDetails", "Services", new {id});
            
        }
        public IActionResult FilteredView(int id)
        {
            string filteredContent = null;

            switch (id)
            {
                case 1:
                    filteredContent = "Construction";
                    break;
                case 2:
                    filteredContent = "Content Creation";
                    break;
                case 3:
                    filteredContent = "Technology & IT Services";
                    break;
                case 4:
                    filteredContent = "Designing and Creativity";
                    break;
                case 5:
                    filteredContent = "Animation & Video Editing";
                    break;
                case 6:
                    filteredContent = "Education & Training";
                    break;
                case 7:
                    filteredContent = "Business & Administrative Services";
                    break;
                case 8:
                    filteredContent = "Healthcare & Wellness";
                    break;
                case 9:
                    filteredContent = "Legal & Financial Services";
                    break;
                case 10:
                    filteredContent = "Home & Lifestyle Services";
                    break;
                case 11:
                    filteredContent = "Repair & Maintenance";
                    break;
                case 12:
                    filteredContent = "Logistics & Delivery";
                    break;
                case 13:
                    filteredContent = "Entertainment & Leisure";
                    break;
                case 14:
                    filteredContent = "Freelance & Gig Services";
                    break;
                case 15:
                    filteredContent = "Digital Marketing";
                    break;
                default:
                    filteredContent = "Unknown Category";
                    break;
            }
           
                var service = c.Services.Where(s=>s.category==filteredContent).ToList();
                return View(service);
            
        }
        public IActionResult UserCart()
        {
            UserCart uc=new UserCart();
            ServicesRepository _sr= new ServicesRepository(c);
            var y = HttpContext.Request.Cookies["currentUser"].Split(',');
            int customerId = Convert.ToInt32(y[0]);
           
                var bookings= c.Bookings.Where(b=>b.customerId==customerId).ToList();
                var SavedServices= c.SavedServicess.Where(s=>s.customerId==customerId).ToList();    
                uc.Bookings = bookings;
                uc.SavedServices = SavedServices;
                uc.exists = _sr.getListOfExist(customerId, bookings);
            
            return View(uc);
        }
        public IActionResult RemoveFromSaved(int id)
        {
            ServicesRepository _Sr = new ServicesRepository(c);
            _Sr.RemoveSavedService(id);
            return View("UserCart");
        }
        public IActionResult RemoveService(int id)
        {
            ServicesRepository _Sr = new ServicesRepository(c);
            _Sr.RemoveService(id);
            return RedirectToAction("ProviderPanel", "Provider");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            TempData["ServiceId"] = id;
           
                var y= c.Services.First(b=>b.id==id);
                return View(y);
            
        }
        [HttpPost]
        public IActionResult Edit(string Title, string Description , float Price)
        {
            ServicesRepository _Sr = new ServicesRepository(c);
            int id = Convert.ToInt32(TempData["ServiceId"]);
            _Sr.EditService(id, Title, Description, Price);

            return RedirectToAction("ProviderPanel", "Provider");
        }
        public IActionResult SortByPricing()
        {
         
                var service = c.Services
                               .OrderBy(s => s.pricing) 
                               .ToList();
                return View(service);
            

        }

        [HttpGet]
        public IActionResult SearchByTitle(string title)
        {
            var services = c.Services
                                   .Where(s => s.title.Contains(title))
                                   .ToList();
            return PartialView("_serviceItem", services);
        }



    }
}
