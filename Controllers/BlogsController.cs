using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async  Task<IActionResult> GetBlogs()
        {
            return Ok(await _DbBlogsContext.Blogs.ToListAsync());
        }

            // gets a blog with its id     
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetBlog([FromRoute] int id)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound();
            }
            return Ok(blog);
        }

        // post(add) a new blog
        [HttpPost]
        public async Task<IActionResult> Create(CreateBlogRequest newBlog)
        {
            var Blog = new Blog()
            {
                Content = newBlog.Content,
                Image = newBlog.Image,
                Title = newBlog.Title,
            }; 
            await _DbBlogsContext.Blogs.AddAsync(Blog);
            
            await _DbBlogsContext.SaveChangesAsync();

            return Ok(Blog);

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound();
            }
            _DbBlogsContext.Remove(blog);
            await _DbBlogsContext.SaveChangesAsync();
            return Ok(blog);


        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateBlog([FromRoute]int id, UpdateBlogRequest updatedBlog)
        {
            var blog = await  _DbBlogsContext.Blogs.FindAsync(id);
                        
                // si le blog Id n'existe pas
            if(blog == null)
            {
                return NotFound();  
            }

                // on peut faire un mise à jour juste sur des colonnes specifiques
            if(updatedBlog.Title != null)
            {
                blog.Title = updatedBlog.Title; 
            }
            if (updatedBlog.Content != null)
            {
                blog.Content = updatedBlog.Content;
            }
            if(updatedBlog.Image != null)
            {
                blog.Image = updatedBlog.Image; 
            }

            await _DbBlogsContext.SaveChangesAsync();

            return Ok(blog);

        }
    }
}
