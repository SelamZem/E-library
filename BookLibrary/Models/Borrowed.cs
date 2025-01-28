using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Models
{
    public class Borrowed
    {
        public int BorrowedId { get; set; }

        [Required]
        public int BookId { get; set; }  

        [Required]
        public int UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        public DateTime ReturnDate { get; set; }


        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [ForeignKey("UserId")]
        public UserAcc UserAcc { get; set; }



    }
}
