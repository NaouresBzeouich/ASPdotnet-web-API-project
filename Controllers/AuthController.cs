using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Project_back_end.JwtBearerConfig;
using Project_back_end.Models;

namespace Project_back_end.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly JwtBearerTokenSettings jwtBearerTokenSettings;
        private readonly UserManager<User> userManager;
        public AuthController(IOptions<JwtBearerTokenSettings> jwtTokenOptions, UserManager<User> userManager)
        {
            this.jwtBearerTokenSettings = jwtTokenOptions.Value;
            this.userManager = userManager;
        }


        [HttpPost]
        [Route("api/Register")]
        public async Task<IActionResult> Register([FromBody] RegisterCredentials userDetails)
        {
            if (!ModelState.IsValid || userDetails == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = 500,
                    Message = "User Registration Failed"
                });
            }
            var identityUser = new User()
            {
                UserName = userDetails.username,
                Email = userDetails.Email,
                Bio = userDetails.Bio ?? ""
            };
            var result = await userManager.CreateAsync(identityUser, userDetails.Password);
            if (!result.Succeeded)
            {
                var dictionary = new ModelStateDictionary();
                foreach (IdentityError error in result.Errors)

                {
                    dictionary.AddModelError(error.Code, error.Description);
                }
                return new BadRequestObjectResult(new
                {
                    Message = "User Registration Failed",
                    Errors =
                dictionary
                });
            }
            return Ok(new { Message = "User Reigstration Successful" });
        }


        [HttpPost]
        [Route("api/Login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
        {
            User identityUser;

            if (!ModelState.IsValid
            || credentials == null
            || (identityUser = await ValidateUser(credentials)) == null)
            {
                return new BadRequestObjectResult(new
                {
                    Message = "Login failed"
                });
            }
            var token = await GenerateToken(identityUser);
            return Ok(new { Token = token, Message = "Success", userId = identityUser.Id });
        }

        [HttpPost]
        [Route("api/Logout")]
        public async Task<IActionResult> Logout()
        {
            // Well, What do you want to do here ?
            // Wait for token to get expired OR
            // Maintain token cache and invalidate the tokens after logout method
            return Ok(new { Token = "", Message = "Logged Out" });
        }
        private async Task<User> ValidateUser(LoginCredentials credentials)
        {
            var identityUser = await
            userManager.FindByNameAsync(credentials.Username);
            if (identityUser != null)
            {
                var result =
                userManager.PasswordHasher.VerifyHashedPassword(identityUser,
                identityUser.PasswordHash, credentials.Password);
                return result == PasswordVerificationResult.Failed ? null : identityUser;
            }
            return null;
        }
        private async Task<object> GenerateToken(User identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var userRoles = await userManager.GetRolesAsync(identityUser); // fetch user's roles

            var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, identityUser.UserName.ToString()),
                        new Claim(ClaimTypes.Email, identityUser.Email)
                    }
                    .Concat(userRoles.Select(role => new Claim(ClaimTypes.Role, role))) // Add roles as separate claims
                ),
                Expires = DateTime.UtcNow.AddSeconds(jwtBearerTokenSettings.ExpireTimeInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtBearerTokenSettings.Audience,
                Issuer = jwtBearerTokenSettings.Issuer
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}