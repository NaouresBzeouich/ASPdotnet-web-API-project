using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Project_back_end.Models
{
    public class User : IdentityUser
    {
        public string? Bio { get; set; }
        public DateTimeOffset JoinDate { get; set; } = DateTimeOffset.Now;
        // User has blogs :
        [JsonIgnore]
        public virtual ICollection<Blog>? Blogs { get; set; }
        // User has comments :


    }
}
