using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using logindemo.Models;

namespace logindemo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ReturnUrl { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(Input.Email);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            if (!string.IsNullOrEmpty(user.SessionToken))
            {
                TempData["AlreadyLoggedIn"] = true;
                TempData["UserId"] = user.Id;
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");

                user.SessionToken = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                var email = user.Email?.ToLower();

                if (roles.Contains("Admin") || email == "cctns@cctns.com")
                    return LocalRedirect("/tickets/AdminDashboard");

                if (email == "vignesh@cctns.com" || email == "pushparaj@cctns.com")
                    return LocalRedirect("/tickets/Home/UserDashboard");

                if (roles.Contains("User"))
                    return LocalRedirect("/tickets/UserDashboard");

                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostLogoutOthersAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Clear old session token
                user.SessionToken = null;
                await _userManager.UpdateAsync(user);
            }

            // Redirect back to login form so they can sign in again
            TempData["LogoutSuccess"] = "All other sessions have been logged out. You can now sign in.";
            return RedirectToPage("./Login");
        }
    }
}
