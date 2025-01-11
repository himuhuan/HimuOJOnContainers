// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace Identity.Server.Pages.Device
{
    [SecurityHeaders]
    [Authorize]
    public class SuccessModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}