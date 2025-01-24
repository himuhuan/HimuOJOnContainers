// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

// Copyright (c) HimuOJ, Apache License 2.0

#region

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

#endregion

namespace Identity.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        [NotMapped]
        public string Subject => Id;

        public string Avatar { get; set; } = DEFAULT_AVATAR;
        public string Background { get; set; } = DEFAULT_BACKGROUND;
        public DateOnly RegisterDate { get; set; }
        public DateOnly LastLoginDate { get; set; }

        #region Constants

        public const string DEFAULT_AVATAR = "default_avatar.png";
        public const string DEFAULT_BACKGROUND = "";

        #endregion
    }
}