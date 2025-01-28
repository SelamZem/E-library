using BookLibrary.Data;
using BookLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDb _bookDb;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(ApplicationDb bookDb, IWebHostEnvironment webHostEnvironment)
        {
            _bookDb = bookDb;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var books = _bookDb.Books.ToList();
            return View(books);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Add(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }

          
            var existingBook = await _bookDb.Books
                .FirstOrDefaultAsync(b => b.Title == book.Title && b.Language == book.Language);

            if (existingBook != null)
            {
                
                return RedirectToAction("Add", "Book");
               
            }

            
            if (book.BImgFile != null && book.BImgFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(book.BImgFile.FileName);
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/books");
                string filePath = Path.Combine(uploadsFolder, fileName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await book.BImgFile.CopyToAsync(stream);
                }

                book.BImgPath = "/images/books/" + fileName;
            }
            else
            {
                book.BImgPath = "/images/books/default.jpg";
            }

            _bookDb.Books.Add(book);
            await _bookDb.SaveChangesAsync();

            return RedirectToAction("Index", "Book");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            
            var book = _bookDb.Books.FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound(); 
            }

            return View(book); 
        }

        [HttpPost]
        public IActionResult Edit(Book model, IFormFile? BImgFile)
        {


            
            var book = _bookDb.Books.FirstOrDefault(b => b.BookId == model.BookId);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return NotFound(); 
            }

            
            book.Title = model.Title;
            book.Author = model.Author;
            book.Publisher = model.Publisher;
            book.Language = model.Language;
            book.Quantity = model.Quantity;

            
            if (BImgFile != null)
            {
                Console.WriteLine("Processing new image upload...");
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "books");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BImgFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    BImgFile.CopyTo(stream);
                }

                book.BImgPath = $"images/books/{fileName}";
            }


          
            try
            {
               
                _bookDb.SaveChanges();
             
            }
            catch (Exception ex)
            {
               
                return View(model);
            }

    
            return RedirectToAction("Index", "Book");
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookDb.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        
        [HttpPost, ActionName("Delete")]
    
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookDb.Books.FindAsync(id);
            if (book != null)
            {
                _bookDb.Books.Remove(book);
                await _bookDb.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Book");
        }
        public IActionResult Views(string search, string language)
        {
            var books = _bookDb.Books.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            if (!string.IsNullOrEmpty(language))
            {
                books = books.Where(b => b.Language == language);
            }

            return View("Views", books.ToList());
        }




    }
}