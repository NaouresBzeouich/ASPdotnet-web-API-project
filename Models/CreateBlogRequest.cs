namespace Project_back_end.Models
{
    public class CreateBlogRequest
    {
        public string? Content { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string UserId { get; set; }
        public int? CategorieId { get; set; }
        public CreateBlogRequest() { }

    }
}
