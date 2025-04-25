using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using FixItNow.Models;
using FixItNow.Models.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FixItNow.Data;

namespace FixItNow.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext c;

        public AuthenticationController(IWebHostEnvironment env, ApplicationDbContext d)
        {
            _webHostEnvironment = env;
            c=d; 
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Home()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(string firstName, string lastName, string email, string phoneNumber,string address, string city ,string gender, string pas)
        {
            AuthenticationRepository ar= new AuthenticationRepository(c);
            if(ar.isAlreadyExists(email)==false)
            {
               
                    User u = new User { address = address, firstName = firstName, lastName = lastName, email = email, phoneNumber = phoneNumber, city = city, gender = gender, password = pas, joinedTime = DateTime.Now };
                    c.Userss.Add(u);
                    Credentials cred = new Credentials { email = email, password = pas, type = "user" };
                    c.Credentialss.Add(cred);
                    c.SaveChanges();
                
                return View("SignIn");
            }
            else
            {
                ViewBag.Message = "email already associated with a person";
                return View();
            }
          
        }
        [HttpPost]
        public IActionResult SignIn(string email,string password)
        {
            if(email=="admin@fixitnow.com" && password=="adminfixitnow")
            {
                return RedirectToAction("Index", "Admin");
            }
            var data = c.Credentialss.Where(cred => cred.email == email && cred.password == password).FirstOrDefault();
            int id = -1;
            if (data != null)
            {
                if (data.type == "user")
                {

                    var data1 = c.Userss.Where(u => u.email == email).FirstOrDefault();
                    id = data1.id;
                }
                else
                {
                    var data1 = c.Providers.Where(p => p.Email == email).FirstOrDefault();
                    id = data1.Id;
                }
            }


            if (data != null)
            {
                Console.WriteLine("success," + data.type);
                HttpContext.Response.Cookies.Append("currentUser", id + "," + data.type);
                if (data.type == "user")
                {
                    return View("Index");
                }
                else
                {
                    return View("ProviderUI");
                }

            }
            else
            {
                ViewBag.x = "incorrect password";
                return View();
            }




        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ProviderRegistration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ProviderRegistration( string FirstName, string LastName, string Email, string Password,string RetypePassword, string Phone,string Address, string City,string Province, string CNIC,IFormFile Resume,IFormFile ProfilePicture, string Education,string skill,string about,string experience)
        {
            if (Password != RetypePassword)
            {
                ModelState.AddModelError("password", "Passwords do not match.");
                return View();
            }
            string wwwPath = this._webHostEnvironment.WebRootPath;
            string uploadPath = Path.Combine(wwwPath, "uploads"); 
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string resumePath = null;
            string profilePicPath = null;

            if (Resume != null)
            {
                var resumeFileName = Path.GetFileNameWithoutExtension(Resume.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(Resume.FileName);
                string relativeResumePath = Path.Combine("uploads", "resumes", resumeFileName).Replace("\\", "/");
                string fullResumePath = Path.Combine(wwwPath, "uploads", "resumes", resumeFileName);

                if (!Directory.Exists(Path.Combine(wwwPath, "uploads", "resumes")))
                {
                    Directory.CreateDirectory(Path.Combine(wwwPath, "uploads", "resumes"));
                }

                using (var stream = new FileStream(fullResumePath, FileMode.Create))
                {
                    Resume.CopyTo(stream);
                }
                resumePath = relativeResumePath;
            }
            if (ProfilePicture != null)
            {
                var profilePicFileName = Path.GetFileNameWithoutExtension(ProfilePicture.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ProfilePicture.FileName);
                string relativeProfilePicPath = Path.Combine("uploads", "profilePics", profilePicFileName).Replace("\\", "/");
                string fullProfilePicPath = Path.Combine(wwwPath, "uploads", "profilePics", profilePicFileName);

                if (!Directory.Exists(Path.Combine(wwwPath, "uploads", "profilePics")))
                {
                    Directory.CreateDirectory(Path.Combine(wwwPath, "uploads", "profilePics"));
                }

                using (var stream = new FileStream(fullProfilePicPath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(stream);
                }
                profilePicPath = relativeProfilePicPath;
            }


            AuthenticationRepository ar = new AuthenticationRepository(c);
            if (ar.isAlreadyExists(Email) == false)
            {
                    var provider = new Provider
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Password = Password,
                        PhoneNumber = Phone,
                        Address = Address,
                        City = City,
                        Province = Province,
                        CNIC = CNIC,
                        ResumePath = resumePath,
                        ProfilePicPath = profilePicPath,
                        Education = Education,
                        status = "UnApproved",
                        about_me = about,
                        expereince = experience,
                        skill = skill
                    };
                    c.Providers.Add(provider);
                    c.SaveChanges();
                }
            else
            {
                ViewBag.Message = "email already associated with a person";
                return View();
            }
            Credentials c1 = new Credentials { email = Email, password = Password, type = "provider" };
            c.Credentialss.Add(c1);
            c.SaveChanges();
            return View("SignIn");
        }
        public IActionResult ProviderUI()
        {
            return View();  
        }
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("currentUser");
            return RedirectToAction("Index", "Home");
        }
    }
}
