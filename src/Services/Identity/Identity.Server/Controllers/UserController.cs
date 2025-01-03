using Identity.Server.Data;
using Identity.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Server.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, IdentityDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

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
                Avatar = user.Avatar
            });
        }

        [HttpGet("briefs")]
        [ProducesResponseType<Dictionary<string, UserBrief>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserBriefs([FromQuery] GetUserBriefsRequest request)
        {
            var ids = request.Id.ToList();

            //NOTE: If the number of Ids in the request is huge, the following query performance may be poor.
            Dictionary<string, UserBrief> queryResult = await _context.Users.AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new { p.Id, UserName = p.UserName ?? "(null)", p.Avatar })
                .ToDictionaryAsync(
                    d => d.Id,
                    d => new UserBrief
                    {
                        UserName = d.UserName,
                        Avatar = d.Avatar
                    }
                );
            return Ok(queryResult);
        }
    }
}