namespace Project_back_end.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public int? likes { get; set; }
        public int? Dislikes { get; set; }
		public virtual User User { get; set; }
		public int? UserId { get; set; }
        public virtual Blog Blog { get; set; }
        public int? BlogId { get; set; }
        public Comment()
        {
            likes = 0;
            Dislikes = 0;
        }


	}
}