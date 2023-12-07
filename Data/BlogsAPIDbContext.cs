using Microsoft.EntityFrameworkCore;
using Project_back_end.Models; 

namespace Project_back_end.Data
{
    public class BlogsAPIDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public BlogsAPIDbContext(DbContextOptions<BlogsAPIDbContext> options) : base(options)
        {
            // Constructor logic
        }
    }
}
