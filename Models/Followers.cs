using System.ComponentModel.DataAnnotations;

namespace Project_back_end.Models
{
    public class followers
    {
        [Key]
        public Guid Id { get; set; }
        public string FollowerId { get; set; }
        public string FollowerUsername { get; set; }

        public string FollowingId { get; set; }
        public string FollowingUsername { get; set; }


    }
}
