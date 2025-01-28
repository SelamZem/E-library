namespace BookLibrary.Models
{
    public class BookReviewViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public double AverageRating { get; set; }
    }
}
