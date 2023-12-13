﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_back_end.Data;
using Project_back_end.Models;

namespace Project_back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly BlogsAPIDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, BlogsAPIDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound(); // User with the given ID not found
        }

        [HttpGet("{id}/blogs")]
        public async Task<IActionResult> GetUserBlogs(string id)
        {
            var user = await _dbContext.users.Include(u => u.Blogs).FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                return Ok(user.Blogs);
            }

            return NotFound(); // User with the given ID not found
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid) // Validate the model state
            {
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "User created successfully." });
                }

                return BadRequest(new { Errors = result.Errors });
            }

            return BadRequest(ModelState); // Return validation errors
        }

        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser != null)
            {
                existingUser.UserName = updatedUser.UserName;
                existingUser.Email = updatedUser.Email;
                existingUser.EmailConfirmed = updatedUser.EmailConfirmed;
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.PhoneNumberConfirmed = updatedUser.PhoneNumberConfirmed;
                existingUser.TwoFactorEnabled = updatedUser.TwoFactorEnabled;
                existingUser.LockoutEnd = updatedUser.LockoutEnd;
                existingUser.LockoutEnabled = updatedUser.LockoutEnabled;
                existingUser.AccessFailedCount = updatedUser.AccessFailedCount;

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return Ok(existingUser); // Return the updated user
                }
                return BadRequest(new { Errors = result.Errors });
            }

            return NotFound(new { Message = "User not found" }); // User with the given ID not found
        }


        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User deleted successfully." });
            }

            return BadRequest(new { Errors = result.Errors });
        }

        [HttpPost("user/{userId}/role/{roleName}")]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Check if the role exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                return NotFound("Role not found");
            }

            // Assign the user to the role
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok($"User {user.UserName} assigned to role {roleName} successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpDelete("user/{userId}/role/{roleName}")]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Check if the user is currently in the role
            var isInRole = await _userManager.IsInRoleAsync(user, roleName);

            if (!isInRole)
            {
                return BadRequest($"User {user.UserName} is not in role {roleName}");
            }

            // Remove the user from the role
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok($"User {user.UserName} removed from role {roleName} successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


    }
}
