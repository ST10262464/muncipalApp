using Microsoft.AspNetCore.Mvc;
using Prog7312_App.Models;
using Prog7312_App.Models.DataStructures;
using System.Security.Cryptography;
using System.Text;

namespace Prog7312_App.Controllers
{
    public class AccountController : Controller
    {
        // Using custom data structures as per the existing pattern in the app
        private static readonly CustomHashTable<string, User> _users = new CustomHashTable<string, User>();
        private static int _nextUserId = 1;

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find user by email
            var user = FindUserByEmail(model.Email);
            if (user != null && VerifyPassword(model.Password, user.Password))
            {
                // Set session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FullName);
                
                // Update last login
                user.LastLoginAt = DateTime.Now;

                // Redirect to return URL or home
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                
                TempData["SuccessMessage"] = $"Welcome back, {user.FirstName}!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if user already exists
            if (FindUserByEmail(model.Email) != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            // Create new user
            var user = new User
            {
                Id = _nextUserId++,
                Email = model.Email.ToLower(),
                Password = HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            // Store user using custom hash table
            _users.Add(user.Email, user);

            // Auto-login after registration
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);

            TempData["SuccessMessage"] = $"Welcome to Municipal Services Portal, {user.FirstName}! Your account has been created successfully.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = FindUserByEmail(userEmail);
            
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Helper methods
        private User? FindUserByEmail(string email)
        {
            if (_users.TryGetValue(email.ToLower(), out User? user))
            {
                return user;
            }
            return null;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "MunicipalSalt2024"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashToCheck = HashPassword(password);
            return hashToCheck == hashedPassword;
        }

        // API endpoint to check if user is logged in (for AJAX calls)
        [HttpGet]
        public IActionResult IsLoggedIn()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return Json(new { isLoggedIn = !string.IsNullOrEmpty(userId) });
        }
    }
}
