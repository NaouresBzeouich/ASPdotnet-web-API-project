namespace Project_back_end.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public int? rating { get; set; }
		public virtual User User { get; set; }
		public int? UserId { get; set; }
        public virtual Blog Blog { get; set; }
        public int? BlogId { get; set; }



	}
}