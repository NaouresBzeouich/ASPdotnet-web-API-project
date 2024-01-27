namespace Project_back_end.Models
{
    public class CreateCommentRequest
    {
        public string? Content { get; set; }
        public Guid? BlogId { get; set; }
        public Guid? UserId { get; set; }

       


    }
}
