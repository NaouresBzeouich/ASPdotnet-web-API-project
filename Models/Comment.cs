using System.ComponentModel.DataAnnotations.Schema;

namespace Project_back_end.Models

{
    public class Comment
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }
        public virtual User Author { get; set; }
        [ForeignKey ("Author")]
        public String? UserId { get; set; }
        public virtual Blog AssociatedBlog { get; set; }
        [ForeignKey ("AssociatedBlog")]
        public Guid? BlogId { get; set; }
        public Comment()
        {
            Likes = 0;
            Dislikes = 0;
        }
        public DateTime? creationDate { get; set; }  

    }
}