// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#region

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace Identity.Server.Pages.Ciba
{
    [AllowAnonymous]
    [SecurityHeaders]
    public class IndexModel : PageModel
    {
        private readonly IBackchannelAuthenticationInteractionService
            _backchannelAuthenticationInteraction;

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IBackchannelAuthenticationInteractionService
                backchannelAuthenticationInteractionService,
            ILogger<IndexModel> logger)
        {
            _backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
            _logger                               = logger;
        }

        public BackchannelUserLoginRequest LoginRequest { get; set; } = default!;

        public async Task<IActionResult> OnGet(string id)
        {
            var result =
                await _backchannelAuthenticationInteraction.GetLoginRequestByInternalIdAsync(id);
            if (result == null)
            {
                _logger.InvalidBackchannelLoginId(id);
                return RedirectToPage("/Home/Error/Index");
            }

            LoginRequest = result;

            return Page();
        }
    }
}