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
    public class LoginModel : PageModel
    {
        // ----------------------------------------------------------------------------------

        #region Variables

        //// Logging: using Microsoft.AspNetCore.Identity custom
        //private readonly SignInManager<IdentityUser> _signInManager;
        //public LoginModel(SignInManager<IdentityUser> signInManager)
        //{
        //    _signInManager = signInManager;
        //}

        private readonly CustomAuthService _auth;
        public LoginModel(CustomAuthService auth)
        {
            _auth = auth;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        
        #endregion

        // ----------------------------------------------------------------------------------

        #region Public Functions

        //public async Task OnGet()
        public void OnGet()
        {
            ReturnUrl = Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ReturnUrl = Url.Content("~/");

            if (ModelState.IsValid)
            {
                //var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, isPersistent: false, lockoutOnFailure: false); // isPersistent: true if you want user to stay logged in if exiting site

                //if (result.Succeeded)
                //{
                //    return LocalRedirect(ReturnUrl);
                //}

                ClaimsPrincipal principal;
                _auth.Users.TryGetValue(Input.UserName, out principal);

                if (principal == null)
                    return Page();

                var identity = principal.Identity as ClaimsIdentity;
                var userName = identity.FindFirst(ClaimTypes.Name)?.Value;
                var password = identity.FindFirst("password")?.Value;

                if (userName == Input.UserName && password == Input.Password)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return LocalRedirect(ReturnUrl);
                }
            }

            return Page();
        }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            //[Required]
            //[EmailAddress]
            //public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public string Role { get; set; }
        }

        #endregion

        // ----------------------------------------------------------------------------------
    }
}
