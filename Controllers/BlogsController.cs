using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;

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

        // a IEnumerable of all the blogs 
        [HttpGet]
        public IActionResult GetBlogs()
        {
            return Ok(_DbBlogsContext.Blogs.ToList());
        }

    }
}
