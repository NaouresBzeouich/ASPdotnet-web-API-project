using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_back_end.Models;

namespace Project_back_end.Data
{
    public class BlogsAPIDbContext : IdentityDbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Categorie> Categories { get; set; }

        public DbSet<User> users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public BlogsAPIDbContext(DbContextOptions<BlogsAPIDbContext> options) : base(options)
        {
            // Constructor logic
        }

    }
}
