using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Project_back_end.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        //private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            //_context = context;
            _roleManager = roleManager;
        }

        [HttpGet("")]
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

        [HttpPost("role/{roleName}")]
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


        [HttpPut("role/{id}/{newRoleName}")]
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



        [HttpDelete("role/{id}")]
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
    }
}

