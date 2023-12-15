namespace Project_back_end.Models
{
    public class UpdateBlogRequest
    {
        public string? Content { get; set; }
        public string? Title { get; set; }
        public int? CategorieId { get; set; }


    }
}