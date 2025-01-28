using BookLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Data
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }

        public DbSet<UserAcc> UsersAcc { get; set; }
        public object UserAcc { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

    
            modelBuilder.Entity<UserAcc>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<UserAcc>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Borrowed> BorrowedList { get; set; }
        public object Borrowed { get; internal set; }
    }
}
