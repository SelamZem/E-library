using BookLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookLibrary.Data;
using System;
using System.Linq;

namespace BookLibrary.Controllers
{
    public class BorrowedController : Controller
    {
        private readonly ApplicationDb _borrowedDb;

        public BorrowedController(ApplicationDb borrowedDb)
        {
            _borrowedDb = borrowedDb;
        }

        public IActionResult Index()
        {
         
            var userIdCookie = Request.Cookies["UserId"]; 
            if (string.IsNullOrWhiteSpace(userIdCookie))
            {
                return Unauthorized("You must be logged in to view borrowed books.");
            }

            int userId = int.Parse(userIdCookie);

            
            var borrowedBooks = _borrowedDb.BorrowedList
                .Where(b => b.UserId == userId)
                .Include(b => b.Book) 
                .Select(b => new BorrowedBookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Book.Title,
                    Author = b.Book.Author,
                    DaysLeft = (b.ReturnDate - DateTime.Now).Days
                })
                .ToList();
            if (borrowedBooks == null || !borrowedBooks.Any())
            {
               
                ViewBag.Message = "No borrowed books found.";
            }
            return View(borrowedBooks);
        }



        [HttpPost]
        public IActionResult BorrowBook(int bookId)
        {
            if (bookId <= 0)
            {
                TempData["Error"] = "Invalid Book ID.";
                return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
            }

            var book = _borrowedDb.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
            }

            if (book.Quantity <= 0)
            {
                TempData["Error"] = "The book is currently unavailable for borrowing.";
                return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
            }

            try
            {
                var userIdCookie = Request.Cookies["UserId"]; 

                if (string.IsNullOrWhiteSpace(userIdCookie))
                {
                    return Unauthorized("You must be logged in to borrow a book.");
                }

                int userId = int.Parse(userIdCookie);

             
                var borrowedBooksCount = _borrowedDb.BorrowedList
                    .Count(b => b.UserId == userId && b.ReturnDate > DateTime.Now);
                if (borrowedBooksCount >= 5)
                {
                    TempData["Error"] = "You can only borrow up to 5 books at a time.";
                    return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
                }

                var existingBorrowedRecord = _borrowedDb.BorrowedList
                    .FirstOrDefault(b => b.UserId == userId && b.BookId == bookId && b.ReturnDate > DateTime.Now); // Only check for books that haven't been returned

                if (existingBorrowedRecord != null)
                {
                    TempData["Error"] = "You have already borrowed this book.";
                    return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
                }
                var borrowed = new Borrowed
                {
                    BookId = book.BookId,
                    UserId = userId,
                    BorrowDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(7) 
                };

                _borrowedDb.BorrowedList.Add(borrowed);

         
                book.Quantity -= 1;
                _borrowedDb.SaveChanges();


                TempData["Message"] = $"You have successfully borrowed the book. Please return it by {borrowed.ReturnDate.ToShortDateString()}.";
                return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                TempData["Error"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
            }
        }


        [HttpPost]
        public IActionResult ReturnBook(int bookId)
        {
            if (bookId <= 0)
            {
                TempData["Error"] = "Invalid Book ID.";
                return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
            }

            try
            {
              
                var userIdCookie = Request.Cookies["UserId"];
                if (string.IsNullOrWhiteSpace(userIdCookie))
                {
                    return Unauthorized("You must be logged in to return a book.");
                }

                int userId = int.Parse(userIdCookie);

                
                var borrowedRecord = _borrowedDb.BorrowedList
                    .FirstOrDefault(b => b.BookId == bookId && b.UserId == userId);

                if (borrowedRecord == null)
                {
                    TempData["Error"] = "No record found for this book borrowed by you.";
                    return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
                }

               
                var book = _borrowedDb.Books.FirstOrDefault(b => b.BookId == bookId);
                if (book == null)
                {
                    TempData["Error"] = "Book not found.";
                    return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
                }

               
                book.Quantity += 1;
                _borrowedDb.BorrowedList.Remove(borrowedRecord);
                _borrowedDb.SaveChanges();
                TempData["Message"] = "You have successfully returned the book.";
                return RedirectToAction("UserA", "UserAcc");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                TempData["Error"] = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("UserA", "UserAcc", new { bookId = bookId });
            }
        }

    }
}
