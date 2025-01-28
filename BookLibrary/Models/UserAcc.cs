using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Models
{
	public class UserAcc
	{
        [Key]
        public int UserId { get; set; }
		[Required(ErrorMessage = "First name is required")]
		[MaxLength(30, ErrorMessage ="Max 30 characters allowed. ")]
        public string FName { get; set; }
        
		[Required(ErrorMessage = "Last name is required")]
        [MaxLength(30, ErrorMessage = "Max 30 characters allowed. ")]
        public string LName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
     
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]

        public string Password { get; set; }
		public string? ImgPath { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public int PhoneNo { get; set; }
		public string? Role { get; set;}
        public string? Keys { get; set;}
    }
}
