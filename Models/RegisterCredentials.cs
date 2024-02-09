using System.ComponentModel.DataAnnotations;

namespace Project_back_end.Models
{
    public class RegisterCredentials
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string email { get; set; }
      //  public string? Bio { get; set; }
    }
}
