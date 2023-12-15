using System.Text.Json.Serialization;

namespace Project_back_end.Models
{
    public class Blog
    {
        // blog Id
        public Guid Id { get; set; }

        // blog contains : title , text(Content) , image and categorie 
        public string? Content { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }

        public int? CategorieId { get; set; }

        // the number of views in this blog
        public int? Views { get; set; }

        // rate associated to the blog Somme(like : +1 ,dislike : -1)
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }

        // association between Blog and its owner ( each bog has only one owner)
        [JsonIgnore]
        public virtual User User { get; set; }
        public Guid? UserId { get; set; }


        // association between Blog and comment : each blog can have 0..* comment(s)


        public Blog()
        {
            Views = 0;
            Likes = 0;
            Dislikes = 0;

        }
    }
}
