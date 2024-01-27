using System.ComponentModel.DataAnnotations;

namespace Project_back_end.Models
{
    public class CommentLikes
    {

        [Key]
        public Guid Id { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? UserId { get; set; }
    }
}
