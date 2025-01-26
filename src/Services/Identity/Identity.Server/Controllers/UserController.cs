#region

using Duende.IdentityServer;
using HimuOJ.Common.BucketStorage;
using HimuOJ.Common.WebHostDefaults.Infrastructure.Auth;
using Identity.Server.Data;
using Identity.Server.Models;
using Identity.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;

#endregion

namespace Identity.Server.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations.
    /// </summary>
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IdentityDbContext _context;
        private readonly IBucketStorage _storage;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserResourcePathService _resourcePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="context">The identity database context.</param>
        /// <param name="storage">The bucket storage service.</param>
        public UserController(
            UserManager<ApplicationUser> userManager,
            IdentityDbContext context,
            IBucketStorage storage,
            UserResourcePathService resourcePath)
        {
            _userManager = userManager;
            _context = context;
            _storage = storage;
            _resourcePath = resourcePath;
        }

        /// <summary>
        /// Gets a brief information of a user by user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The brief information of the user.</returns>
        [HttpGet("{userId}/brief")]
        [ProducesResponseType<UserBrief>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserBrief(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserBrief
            {
                UserName = user.UserName ?? string.Empty,
                Avatar = _resourcePath.GetUserAvatarFullPath(userId, user.Avatar)
            });
        }

        /// <summary>
        /// Gets brief information of multiple users by their IDs.
        /// </summary>
        /// <param name="request">The request containing user IDs.</param>
        /// <returns>A dictionary of user IDs and their brief information.</returns>
        [HttpGet("briefs")]
        [ProducesResponseType<Dictionary<string, UserBrief>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserBriefs([FromQuery] GetUserBriefsRequest request)
        {
            if (request.Ids == null || !request.Ids.Any())
            {
                return NoContent();
            }

            var ids = request.Ids.ToList();

            //NOTE: If the number of Ids in the request is huge, the following query performance may be poor.
            Dictionary<string, UserBrief> queryResult = await _context.Users.AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new { p.Id, UserName = p.UserName ?? "(null)", p.Avatar })
                .ToDictionaryAsync(
                    d => d.Id,
                    d => new UserBrief
                    {
                        UserName = d.UserName,
                        Avatar = _resourcePath.GetUserAvatarFullPath(d.Id, d.Avatar)
                    }
                );
            return Ok(queryResult);
        }

        /// <summary>
        /// Gets detailed information of a user by user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The detailed information of the user.</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType<UserDetail>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserDetail(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserDetail
            {
                UserId = user.Id,
                Avatar = _resourcePath.GetUserAvatarFullPath(userId, user.Avatar),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                RegisterDate = user.RegisterDate.ToShortDateString(),
                LastLoginDate = user.LastLoginDate.ToShortDateString()
            });
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The request containing user information.</param>
        /// <returns>The detailed information of the created user.</returns>
        [HttpPost]
        [ProducesResponseType<UserDetail>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                RegisterDate = DateOnly.FromDateTime(DateTime.UtcNow),
                LastLoginDate = DateOnly.MinValue,
                // TODO: Add email confirmation
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, ApplicationRoleConstants.StandardUser);

            return CreatedAtAction(nameof(GetUserDetail), new { userId = user.Id },
                new UserDetail
                {
                    UserId = user.Id,
                    Avatar = user.Avatar,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    RegisterDate = user.RegisterDate.ToShortDateString(),
                    LastLoginDate = user.LastLoginDate.ToShortDateString()
                });
        }

        /// <summary>
        /// Updates the avatar of a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="file">The avatar file.</param>
        /// <returns>No content if the update is successful.</returns>
        [Authorize(Policy = "PublicApi")]
        [HttpPut("{userId}/avatar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAvatar(string userId, IFormFile file)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            bool success = false;
            try
            {
                await using var stream = file.OpenReadStream();
                var randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                    + Path.GetExtension(Path.GetFileName(file.FileName));
                var path = _resourcePath.GetUserAvatarPath(user.Id, randomFileName);
                await _storage.UploadAsync(stream, path, file.Length, file.ContentType);
                success = true;
                user.Avatar = randomFileName;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (success)
            {
                await _userManager.UpdateAsync(user);
            }

            return NoContent();
        }
    }
}