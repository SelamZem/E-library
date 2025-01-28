using BookLibrary.Data;
using BookLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Controllers
{
	public class UserAccController : Controller
	{
        private readonly ApplicationDb _userDb;
        public UserAccController(ApplicationDb userDb)
        {

            _userDb = userDb;
        }


      
		public IActionResult Register()
		{
			return View();

		}


        [HttpPost]
        public IActionResult Register(UserAcc model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }


      
            if (_userDb.UsersAcc.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email is already in use.");
                return View(model);
            }
            if (_userDb.UsersAcc.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("", "Username is already taken.");
                return View(model);
            }


         
            using (var hmac = new System.Security.Cryptography.HMACSHA256())
            {
                var salt = hmac.Key;
                var hashedPassword = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password)));

             
                var user = new UserAcc
                {
                    FName = model.FName,
                    LName = model.LName,
                    Email = model.Email,
                    UserName = model.UserName,
                    Password = hashedPassword,
                    ImgPath = model.ImgPath,
                    PhoneNo = model.PhoneNo,
                    Role = model.Role,
                    Keys = Convert.ToBase64String(salt) 
                };

                _userDb.UsersAcc.Add(user);
                _userDb.SaveChanges();
            }

            return RedirectToAction("Login", "UserAcc");
        }


        public IActionResult Login()
		{
			return View();
		}

        [HttpPost]
        public IActionResult Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

       
            var user = _userDb.UsersAcc.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email or Password.");
                return View(model);
            }

     
            var salt = Convert.FromBase64String(user.Keys);

            using (var hmac = new System.Security.Cryptography.HMACSHA256(salt))
            {
                var computedHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password)));
                if (user.Password != computedHash)
                {
                    ModelState.AddModelError("", "Invalid Email or Password.");
                    return View(model);
                }
            }

           
            var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),  
                HttpOnly = true,  
                Secure = true,    
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
            };

            HttpContext.Response.Cookies.Append("UserId", user.UserId.ToString(), cookieOptions);



            if (user.Role == "Admin")
            {
                return RedirectToAction("Admin", "UserAcc");
            }
            else
            {
                return RedirectToAction("UserA", "UserAcc");
            }

        }

        public IActionResult Logout()
        {
           
           

            return RedirectToAction("Login", "UserAcc");
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult UserA()
        {
            var books = _userDb.Books.ToList();
            return View("UserA", books);
        }


    }
}
