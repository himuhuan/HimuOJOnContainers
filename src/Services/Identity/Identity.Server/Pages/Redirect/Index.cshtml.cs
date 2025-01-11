// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace Identity.Server.Pages.Redirect
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public string? RedirectUri { get; set; }

        public IActionResult OnGet(string? redirectUri)
        {
            if (!Url.IsLocalUrl(redirectUri))
            {
                return RedirectToPage("/Home/Error/Index");
            }

            RedirectUri = redirectUri;
            return Page();
        }
    }
}