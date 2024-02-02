using System.ComponentModel.DataAnnotations;

namespace Project_back_end.Models
{
    public class ImageModel
    {
        public Guid BlogId { get; set; }
        
        public IFormFile? Image { get; set; }

    
    }
}
