using BookLibrary.Data;
using BookLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace BookLibrary.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDb _reviewDb;

        public ReviewController(ApplicationDb reviewDb)
        {
            _reviewDb = reviewDb;
        }

        // View Book Details and Reviews
        public IActionResult Details(int bookId)
        {
            if (bookId <= 0)
            {
                return BadRequest("Invalid Book ID."); 
            }

            var book = _reviewDb.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound("Book not found."); 
            }

            var reviews = _reviewDb.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.UserAcc)
                .ToList();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            var model = new BookReviewViewModel
            {
                Book = book,
                Reviews = reviews,
                AverageRating = averageRating
            };

            return View(model);
        }

       
        [HttpPost]
        public IActionResult PostReview(int bookId, string commentText, int rating)
        {
            if (bookId <= 0)
            {
                return BadRequest("Invalid Book ID."); 
            }

            if (string.IsNullOrWhiteSpace(commentText))
            {
                return BadRequest("Comment text cannot be empty."); 
            }

            if (rating < 1 || rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5."); 
            }

            try
            {
                var userIdCookie = Request.Cookies["UserId"];
                if (string.IsNullOrWhiteSpace(userIdCookie))
                {
                    return Unauthorized("You must be logged in to post a review."); 
                }

                int userId = int.Parse(userIdCookie);

                var book = _reviewDb.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null)
                {
                    return NotFound("Book not found.");
                }

             
                var existingReview = _reviewDb.Reviews
                    .FirstOrDefault(r => r.BookId == bookId && r.UserId == userId);
                if (existingReview != null)
                {
                    return Conflict("You have already posted a review for this book."); 
                }

                var review = new Review
                {
                    BookId = bookId,
                    CommentText = commentText,
                    Rating = rating,
                    UserId = userId,
                    CommentDate = DateTime.Now
                };

                _reviewDb.Reviews.Add(review);
                _reviewDb.SaveChanges(); 

                return RedirectToAction("Details", "Review", new { bookId });
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return StatusCode(500, "An error occurred while saving the review. Please try again.");
            }
        }


    }
}
