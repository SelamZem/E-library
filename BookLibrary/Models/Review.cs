using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }  // Primary Key for Review

        [Required]
        public int BookId { get; set; }  // Foreign Key for Book

        [Required]
        public int UserId { get; set; }  // Foreign Key for User (this is referencing UserAcc)

        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string CommentText { get; set; }  // The Comment text

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }  // Rating (1-5)

        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; } = DateTime.Now;  // Date of comment

      
        [ForeignKey("BookId")]
        public Book Book { get; set; }  

        [ForeignKey("UserId")]
        public UserAcc UserAcc { get; set; }  
    }
}
