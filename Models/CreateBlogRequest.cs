namespace Project_back_end.Models
{
    public class CreateBlogRequest
    {
        public string? Content { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public Guid? UserId { get; set; }
        public int? CategoryId { get; set; }

        public CreateBlogRequest() { }

    }
}
