using FixItNow.Models;
using FixItNow.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FixItNow.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<MyApplicationUser> _signInManager;
        private readonly UserManager<MyApplicationUser> _userManager;

        public AccountController(
            SignInManager<MyApplicationUser> signInManager,
            UserManager<MyApplicationUser> userManager
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
 
        }

       
        [HttpGet]
        public IActionResult SignIn()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
               
                if (result.Succeeded)
                {
                    Console.WriteLine("here");
                    // Redirect to home page after successful login
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Show error message if credentials are incorrect
                    ViewBag.ErrorMessage = "Invalid login attempt.";
                    return View();
                }
            }
            else
            {
                // User not found
                ViewBag.ErrorMessage = "No account found with that email.";
                return View();
            }
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new MyApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    City = model.City,
                    Gender = model.Gender
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                 
                    await _signInManager.SignInAsync(user, isPersistent: false);

       
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
  
            return View(model);
        }
    }
}
