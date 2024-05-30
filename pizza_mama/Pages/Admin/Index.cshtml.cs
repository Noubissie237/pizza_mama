using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace pizza_mama.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public bool displayInvalidAccountMessage = false;
        IConfiguration configuration;
        public IndexModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult OnGet(string ReturnUrl = "/admin/pizzas")
        {
            if (this.HttpContext.User.Identity.IsAuthenticated)
                return Redirect(ReturnUrl);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string username, string password, string ReturnUrl) 
        {
            var authSection = configuration.GetSection("Auth");

            string adminLogin = authSection["AdminLogin"];
            string adminPassword = authSection["AdminPassword"];

            if (username == adminLogin && password == adminPassword)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
                var claimsIdentity = new ClaimsIdentity(claims, "Login");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity));

                this.displayInvalidAccountMessage = false;
                return Redirect(ReturnUrl == null ? "/Admin/Pizzas" : ReturnUrl);
            }
            this.displayInvalidAccountMessage = true;
            return Page();
        }

        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/admin");
        }
    }
}
