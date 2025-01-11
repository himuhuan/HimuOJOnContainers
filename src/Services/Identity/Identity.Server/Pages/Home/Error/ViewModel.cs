// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

#region

using Duende.IdentityServer.Models;

#endregion

namespace Identity.Server.Pages.Error
{
    public class ViewModel
    {
        public ViewModel()
        {
        }

        public ViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }

        public ErrorMessage? Error { get; set; }
    }
}