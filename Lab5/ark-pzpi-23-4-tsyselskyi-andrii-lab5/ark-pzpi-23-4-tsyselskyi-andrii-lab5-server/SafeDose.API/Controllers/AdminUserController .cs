using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SafeDose.Application.Contracts.Identity;
using SafeDose.Application.Models.Identity.UserService;
using SafeDose.Identity.Models;

namespace SafeDose.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AuthUser> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;

        public AdminUserController(
            IUserService userService,
            UserManager<AuthUser> userManager,
            RoleManager<IdentityRole<long>> roleManager)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = _userManager.Users.ToList();

            var result = new List<object>();

            foreach (var authUser in users)
            {
                var roles = await _userManager.GetRolesAsync(authUser);

                result.Add(new
                {
                    authUser.Id,
                    authUser.Email,
                    authUser.UserName,
                    authUser.PhoneNumber,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.PhoneNumber,
                Roles = roles
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {
            var id = await _userService.CreateAsync(model);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UserModel model)
        {
            await _userService.UpdateAsync(model, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
                return BadRequest(deleteResult.Errors);

            return NoContent();
        }

        [HttpPost("{id}/roles/add")]
        public async Task<IActionResult> AddRole(long id, [FromQuery] string role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost("{id}/roles/remove")]
        public async Task<IActionResult> RemoveRole(long id, [FromQuery] string role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRoles(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
    }
}
