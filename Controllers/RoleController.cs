using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_back_end.Models;

namespace Project_back_end.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found." });
            }
            return Ok(role);
        }

        [HttpPost("{roleName}")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var role = new IdentityRole(roleName);

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Role created successfully." });
            }

            return BadRequest(new { Errors = result.Errors });
        }


        [HttpPut("{id}/{newRoleName}")]
        public async Task<IActionResult> UpdateRole(string id, string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound(new { Message = "Role not found." });
            }

            role.Name = newRoleName;

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Role updated successfully." });
            }

            return BadRequest(new { Errors = result.Errors });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound(new { Message = "Role not found." });
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Role deleted successfully." });
            }

            return BadRequest(new { Errors = result.Errors });
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersInRole(string id)
        {
            // Find the role by name
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                // Handle the case where the role doesn't exist
                return NotFound(new { Message = "Role not found." });
            }

            // Get the users in the specified role
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            return Ok(usersInRole);
        }


    }
}

