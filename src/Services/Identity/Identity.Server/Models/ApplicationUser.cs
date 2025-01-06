// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

// Copyright (c) HimuOJ, Apache License 2.0

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        #region Constants

        public const string DEFAULT_AVATAR = "/static/users/default/default_avatar.png";
        public const string DEFAULT_BACKGROUND = "";

        #endregion

        [NotMapped]
        public string Subject => Id.ToString();

        public string Avatar { get; set; } = DEFAULT_AVATAR;
        public string Background { get; set; } = DEFAULT_BACKGROUND;
        public DateOnly RegisterDate { get; set; }
        public DateOnly LastLoginDate { get; set; }
    }
}
