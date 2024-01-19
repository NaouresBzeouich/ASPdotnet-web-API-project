using System.ComponentModel.DataAnnotations;

namespace Project_back_end.Models
{
    public class BlogDislikes
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
    }
}
