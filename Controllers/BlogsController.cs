using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {

        private readonly BlogsAPIDbContext _DbBlogsContext; 

        public BlogsController(BlogsAPIDbContext DbBlogsContext) 
        {
            this._DbBlogsContext = DbBlogsContext; 
        }

        // gets all the blogs 
        [HttpGet]
        public IActionResult GetBlogs()
        {
            return Ok(_DbBlogsContext.Blogs.ToList());
        }

        // post(add) a new blog
        [HttpPost]
        public IActionResult Create(CreateBlogRequest newBlog)
        {
            var Blog = new Blog()
            {
                Content = newBlog.Content,
                Image = newBlog.Image,
                Title = newBlog.Title,
            }; 
            _DbBlogsContext.Blogs.Add(Blog);
            
            _DbBlogsContext.SaveChanges();

            return Ok(Blog);

        }
    }
}
