using LAGem_POPortal.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LAGem_POPortal.Areas.Identity.Pages.Account
{
    public class RegisterModel: PageModel
    {
        //// Logging: using Microsoft.AspNetCore.Identity custom
        //private readonly SignInManager<IdentityUser> _signInManager;
        //private readonly UserManager<IdentityUser> _userManager;
        //public RegisterModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        //{
        //    _signInManager = signInManager;
        //    _userManager = userManager;
        //}

        private readonly CustomAuthService _auth;
        public RegisterModel(CustomAuthService auth)
        {
            _auth = auth;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        //public async Task OnGet()
        public void OnGet()
        {
            ReturnUrl = Url.Content("~/");
        }

        //public IActionResult OnPost()
        //{
        //    ReturnUrl = Url.Content("~/");
        //    return Page();
        //}

        //private void Submit()
        //{
        //}

        public async Task<IActionResult> OnPostAsync() // not getting Input data
        {
            ReturnUrl = Url.Content("~/");

            if (ModelState.IsValid)
            {
                //// Logging: using Microsoft.AspNetCore.Identity custom
                //var identity = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                //var result = await _userManager.CreateAsync(identity, Input.Password);

                //if (result.Succeeded)
                //{
                //    await _signInManager.SignInAsync(identity, isPersistent: false); // true if you want user to stay logged in if exiting site
                //    return LocalRedirect(ReturnUrl);
                //}

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, Input.UserName));
                claims.Add(new Claim(ClaimTypes.Email, Input.Email));
                claims.Add(new Claim("password", Input.Password));
                claims.Add(new Claim(ClaimTypes.Role, Input.Role));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(claimsIdentity);

                _auth.Users.Add(Input.UserName, principal);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return LocalRedirect(ReturnUrl);
            }

            return Page();
        }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            //[Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public string Role { get; set; }
        }
    }
}
