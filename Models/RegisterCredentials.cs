using System.ComponentModel.DataAnnotations;

namespace Project_back_end.Models
{
    public class RegisterCredentials
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
