using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;

        private readonly BlogsAPIDbContext _DbBlogsContext;

        public BlogController(BlogsAPIDbContext DbBlogsContext, ILogger<BlogController> logger)
        {
            this._logger = logger;
            this._DbBlogsContext = DbBlogsContext;
        }


        // gets all the blogs 
        [HttpGet]
        [Route("/getBlogs")]
        public async Task<IEnumerable<Blog>> GetBlogs()
        {
            var blogs = await _DbBlogsContext.Blogs.ToListAsync();
            return blogs;
        }

        // gets a blog with its id     
        [HttpGet]
        [Route("/getBlog/{id}")]
        public async Task<Blog> GetBlog([FromRoute] Guid id)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return null; // 5alit'ha bech be3id bech terja lel move elli 9bel'ha nrmlmnt avec un error msg 
            }
            return blog;
        }

        // gets the trending blogs (the top 10 blogs that have the biggest likes number  ) 
        [HttpGet]
        [Route("/getTrendingBlogs")]
        public async Task<IEnumerable<Blog>> getTrendingBlogs()
        {
            IEnumerable<Blog> blogs = await _DbBlogsContext.Blogs.OrderByDescending(blog => blog.Likes).Take(10).ToListAsync();
            return blogs;

        }

        // post(add) a new blog
        [HttpPost]
        [Route("/createBlog")]

        public async Task<IActionResult> Create([FromBody] CreateBlogRequest newBlog)
        {
            _logger.LogInformation("inside the action !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            string guidString = newBlog.UserId?.ToString();

            var user = await _DbBlogsContext.users.FirstOrDefaultAsync(x=>(x.Id== guidString));
            try{

                var Blog = new Blog()
                {
                    Content = newBlog.Content,
                    Image = newBlog.Image,
                    Title = newBlog.Title,
                    CategorieId = newBlog.CategoryId,
                    UserId = newBlog.UserId,
                    User = user
                };
                await _DbBlogsContext.Blogs.AddAsync(Blog);


                await _DbBlogsContext.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "",
                    Blog = Blog,
                });



            }


            catch {

                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "bad request in the catch",
                   
                });
                    }
           

        }

        // delete a blog by its id 
        [HttpDelete]
        [Route("/delete/{id}")]
        public async Task<Blog> Delete(Guid id)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return null; // mbe3id bech inredirectiha lel api necessary 
            }
            _DbBlogsContext.Remove(blog);
            await _DbBlogsContext.SaveChangesAsync();
            return blog;


        }

        // update the blog by its id 
        [HttpPut]
        [Route("/updateBlog/{id}")]
        public async Task<Blog> UpdateBlog([FromRoute] Guid id, UpdateBlogRequest updatedBlog)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return null; // mbe3id netfehmou   
            }

            // on peut faire un mise à jour juste sur des colonnes specifiques
            if (updatedBlog.Title != null)
            {
                blog.Title = updatedBlog.Title;
            }
            if (updatedBlog.Content != null)
            {
                blog.Content = updatedBlog.Content;
            }
         /*   if (updatedBlog.Image != null)
            {
                blog.Image = updatedBlog.Image;
            }*/
            if (updatedBlog.CategorieId != null)
            {
                blog.CategorieId = updatedBlog.CategorieId;
            }

            await _DbBlogsContext.SaveChangesAsync();

            return blog;

        }

        // get blogs by its owner id 
        [HttpPost]
        [Route("/getBlogsByUser")]
        public async Task<IEnumerable<Blog>> getBlogsByUser([FromBody] testModel userId)
        {
            Guid id = Guid.Parse(userId.name);
            var blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.UserId == id);
            return blogs;
        }


        // get blogs by category
        [HttpPost]
        [Route("/getBlogsByCategory")]
        public async Task<IEnumerable<Blog>> getBlogsByCategory( Categorie Category)
        {
            IEnumerable<Blog> blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.CategorieId == Category.Id);
            return blogs;

        }

    }
}
