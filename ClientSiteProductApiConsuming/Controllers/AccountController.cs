using ClientSiteProductApiConsuming.Models;
using ClientSiteProductApiConsuming.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientSiteProductApiConsuming.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(); // Make sure this points to Views/Account/Login.cshtml
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLogin loginModel)
        {
            if (ModelState.IsValid)
            {
                // Authenticate and retrieve the token
                string token = await _authService.AuthenticateAsync(loginModel);

                if (!string.IsNullOrEmpty(token))
                {
                    // Store the token in session
                    HttpContext.Session.SetString("JWToken", token);

                    // Redirect to the dashboard on successful login
                    return RedirectToAction("Dashboard", "Products");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(loginModel);
        }
    }
}
