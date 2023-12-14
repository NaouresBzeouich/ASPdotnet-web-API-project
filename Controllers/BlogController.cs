﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;
using Microsoft.EntityFrameworkCore;


namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {

        private readonly BlogsAPIDbContext _DbBlogsContext;

        public BlogController(BlogsAPIDbContext DbBlogsContext)
        {
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
        public async Task<IActionResult> GetBlog([FromRoute] Guid id)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound("there's no blog with this Id");  
            }
            return Ok(blog);
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
            var user = await _DbBlogsContext.users.FindAsync(newBlog.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var Blog = new Blog()
            {
                Content = newBlog.Content,
                Image = newBlog.Image,
                Title = newBlog.Title,
                CategorieId = newBlog.CategorieId,
                UserId = newBlog.UserId,
                User = user
            };
            await _DbBlogsContext.Blogs.AddAsync(Blog);

            await _DbBlogsContext.SaveChangesAsync();

            return Ok(Blog);

        }

        // delete a blog by its id 
        [HttpDelete]
        [Route("/delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound(" no blog with this id is found " ) ; 
            }
            _DbBlogsContext.Remove(blog);
            await _DbBlogsContext.SaveChangesAsync();
            return Ok(blog);


        }

        // update the blog by its id 
        [HttpPut]
        [Route("/updateBlog/{id}")]
        public async Task<IActionResult> UpdateBlog([FromRoute] Guid id, UpdateBlogRequest updatedBlog)
        {
            var blog = await _DbBlogsContext.Blogs.FindAsync(id);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound("no blog with that id found ");   
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
            if (updatedBlog.Image != null)
            {
                blog.Image = updatedBlog.Image;
            }
            if (updatedBlog.CategorieId != null)
            {
                blog.CategorieId = updatedBlog.CategorieId;
            }

            await _DbBlogsContext.SaveChangesAsync();

            return Ok(blog);

        }

        // get blogs by its owner id 
        [HttpPost]
        [Route("/getBlogsByUser/{user}")]
        public async Task<IActionResult> getBlogsByUser([FromRoute] User user)
        {
            var blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.User == user);

            if (blogs == null)
            { return NotFound("there 's no blogs created by this user "); }
            return Ok(blogs);
        }


        // get blogs by category
        [HttpGet]
        [Route("/getBlogsByCategory/{Category}")]
        public async Task<IActionResult> getBlogsByCategory([FromRoute] Categorie Category)
        {
            IEnumerable<Blog> blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.CategorieId == Category.Id);
            if (blogs == null)
                return NotFound("there's no blogs in this category ");
            return Ok(blogs);

        }

    }
}
