using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LAGem_POPortal.Areas.Identity.Pages.Account
{
    public class TestPageModel : PageModel
    {
        public void OnGet()
        {
        }

        public string ReturnUrl { get; set; }
        public IActionResult Submit()
        {
            ReturnUrl = Url.Content("~/");

            return Page();
        }

        public async Task OnPostAsync()
        {
            //< Do Post Stuff Here >
        }
        public async Task OnPostButton()
        {
            //< Do button stuff here >
        }
    }
}