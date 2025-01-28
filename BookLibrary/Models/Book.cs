using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }
        public string Author { get; set; } 
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string? BImgPath { get; set; }
        public int Quantity{ get; set; }

        [NotMapped] // This property is not mapped to the database
        public IFormFile? BImgFile { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

         

    }
}
