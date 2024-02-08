using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly ILogger<BlogController> _logger;
        private readonly IWebHostEnvironment _environment;

        public UserController(UserManager<User> userManager, ILogger<BlogController> logger, RoleManager<IdentityRole> roleManager, BlogsAPIDbContext dbContext, IWebHostEnvironment environment)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound(); // User with the given ID not found
        }


        [HttpPost("getUserCounts")]
        public async Task<IActionResult> getUserCounts(@string id)
        {
            var user = await _userManager.FindByIdAsync(id.name);

            if (user == null)
            {
                return NotFound();
            }

            var followersCount = await _dbContext.Followers.CountAsync(x => x.FollowingId == id.name);
            var followingsCount = await _dbContext.Followers.CountAsync(x => x.FollowerId == id.name);
            var blogs = await _dbContext.Blogs.Where(x => x.UserId == id.name).ToListAsync();
            var blogsCounts = blogs.Count();
            int totalLikes = (int)blogs.Sum(blog => blog.Likes);

            return Ok(new
            {
                followers = followersCount,
                followings = followingsCount,
                blogs = blogsCounts,
                likes = totalLikes


            }); // User with the given ID not found
        }




        /*
                [HttpGet("{id}/blogs")]
                public async Task<IActionResult> GetUserBlogs(string id)
                {
                    var user= await _dbContext.users.Include(u => u.Blogs).FirstOrDefaultAsync(u => u.Id == id);

                    if (user != null)
                    {
                        return Ok(user.Blogs);
                    }

                    return NotFound(); // User with the given ID not found
                }

                [HttpPost]
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
                }*/



        [HttpPut("/updateUser/{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser != null)
            {
                if (updatedUser.UserName != null)
                    existingUser.UserName = updatedUser.UserName;
                if (updatedUser.Email != null)
                    existingUser.Email = updatedUser.Email;
                if (updatedUser.EmailConfirmed != null)
                    existingUser.EmailConfirmed = updatedUser.EmailConfirmed;
                if (updatedUser.PhoneNumber != null)
                    existingUser.PhoneNumber = updatedUser.PhoneNumber;
                if (updatedUser.PhoneNumberConfirmed != null)
                    existingUser.PhoneNumberConfirmed = updatedUser.PhoneNumberConfirmed;
                if (updatedUser.TwoFactorEnabled != null)
                    existingUser.TwoFactorEnabled = updatedUser.TwoFactorEnabled;
                if (updatedUser.LockoutEnd != null)
                    existingUser.LockoutEnd = updatedUser.LockoutEnd;
                if (updatedUser.LockoutEnabled != null)
                    existingUser.LockoutEnabled = updatedUser.LockoutEnabled;
                if (updatedUser.AccessFailedCount != null)
                    existingUser.AccessFailedCount = updatedUser.AccessFailedCount;

                if (updatedUser.Bio != null)
                    existingUser.Bio = updatedUser.Bio;


                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return Ok(existingUser); // Return the updated user
                }
                return BadRequest(new { Errors = result.Errors });
            }

            return NotFound(new { Message = "User not found" }); // User with the given ID not found
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,user")]
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

        [HttpPost("{userId}/role/{roleName}")]
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

        [HttpDelete("{userId}/role/{roleName}")]
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

        [HttpPost]
        [Route("ImageUpload")]
        public async Task<IActionResult> ImageUpload([FromForm] ImageModel imageModel)
        {

            var user = await _userManager.FindByIdAsync(imageModel.BlogId.ToString());

            if (user == null)
            {
                return NotFound("User not found ! ");
            }
            try
            {
                string Filepath = this._environment.WebRootPath + "\\Uploads\\Profiles\\" + imageModel.BlogId;
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }
                string imagepath = Filepath + "\\image.png";
                if (System.IO.File.Exists(imagepath))  // so you can use this also when you wanna update the image 
                {
                    System.IO.File.Delete(imagepath);
                }

                using (Stream stream = new FileStream(imagepath, FileMode.Create))
                {
                    imageModel.Image.CopyTo(stream);
                }

                user.Image = getImageByUser(imageModel.BlogId);
                await _userManager.UpdateAsync(user);

                return Ok(user);

            }
            catch (Exception ex)
            {
                return BadRequest("error in uploading the image !" + ex.Message);
            }

        }

        [HttpDelete]
        [Route("ImageRemove")]
        public async Task<IActionResult> removeImage(string userId)
        {
            string HostURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            string Filepath = _environment.WebRootPath + "\\Uploads\\Profiles\\" + userId;
            string imagepath = Filepath + "\\image.png";
            var user = await _userManager.FindByIdAsync(userId);

            if (System.IO.Directory.Exists(Filepath))
            {
                System.IO.File.Delete(imagepath);
                System.IO.Directory.Delete(Filepath);

                user.Image = HostURL + "commun/noImage.jpg";
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }



        [NonAction]
        private string getImageByUser(Guid userId)
        {
            string HostURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/";
            string Filepath = _environment.WebRootPath + "\\Uploads\\Profiles\\" + userId;
            //_ = Filepath + "\\image.png";

            string imageURL;
            if (!System.IO.Directory.Exists(Filepath))
            {
                imageURL = HostURL + "commun/noImage.jpg";
            }
            else
            {
                imageURL = HostURL + "Uploads/Profiles/" + userId + "/image.png";
            }

            return imageURL;
        }



        [HttpPatch("setBioEmptyString/{id}")]
        public async Task<IActionResult> setBioEmptyString(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)

                return NotFound(); // User with the given ID not found



            user.Bio = "";
            await _dbContext.SaveChangesAsync();
            return Ok();

        }






    }
}
