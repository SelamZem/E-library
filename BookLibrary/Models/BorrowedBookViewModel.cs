namespace BookLibrary.Models
{
    public class BorrowedBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int DaysLeft { get; set; }
        public string BImgPath { get; set; }
    }

}
