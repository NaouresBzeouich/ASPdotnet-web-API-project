using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System;
using System.Data.SqlTypes;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;

        private readonly BlogsAPIDbContext _DbBlogsContext;

        private readonly IWebHostEnvironment _environment;

        public BlogController(BlogsAPIDbContext DbBlogsContext, ILogger<BlogController> logger, IWebHostEnvironment _environment)
        {
            this._logger = logger;
            this._DbBlogsContext = DbBlogsContext;
            this._environment = _environment;
        }


        // gets all the blogs 
        [HttpGet]
        [Route("/getBlogs")]
        public async Task<IEnumerable<Blog>> GetBlogs()
        {
            var blogs = await _DbBlogsContext.Blogs.ToListAsync();
            if (blogs != null && blogs.Count > 0)
            {
                foreach (var blog in blogs)
                {
                    blog.Image = getImageByBlog(blog.Id);
                }

                await _DbBlogsContext.SaveChangesAsync();

                return blogs;
            }
            else
            {
                return new List<Blog>();
            }
        }

        // gets a blog with its id     
        [HttpPost]
        [Route("/getBlog")]
        public async Task<IActionResult> GetBlog([FromBody] @string id)
        {
            Guid guid = Guid.Parse(id.name);
            //_logger.LogInformation(guid);


            //   var blog = await _DbBlogsContext.Blogs.FindAsync(guid);
            var blog = await _DbBlogsContext.Blogs.FirstOrDefaultAsync(x => x.Id == guid);


            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound(); // 5alit'ha bech be3id bech terja lel move elli 9bel'ha nrmlmnt avec un error msg 
            }

            blog.Image = getImageByBlog(blog.Id);
            await _DbBlogsContext.SaveChangesAsync();

            return Ok(blog)
                    ;
        }



        [HttpPost]
        [Route("/getBlogsByUserId")]
        public async Task<IActionResult> GetBlogsByUserId([FromBody] @string id)
        {
            Guid guid = Guid.Parse(id.name);
            //_logger.LogInformation(guid);


              var blogs = await _DbBlogsContext.Blogs.Where(x => x.UserId == id.name).OrderByDescending(blog => blog.CreationDate).ToListAsync();

            // si le blog Id n'existe pas
            if (blogs == null)
            {
                return NotFound(new
                {

                    Message = "no blogs found !"


                }
                    ); // 5alit'ha bech be3id bech terja lel move elli 9bel'ha nrmlmnt avec un error msg 
            }

            await _DbBlogsContext.SaveChangesAsync();

            return Ok(blogs)
                    ;
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
            // _logger.LogInformation("inside the action !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            string guidString = newBlog.UserId?.ToString();
            Guid guidid = Guid.Parse(newBlog.UserId);

            //     var user = await _DbBlogsContext.users.FirstOrDefaultAsync(x => (x.Id == guidString));
            try
            {

                var Blog = new Blog()
                {
                    Content = newBlog.Content,
                    Image = newBlog.Image,
                    Title = newBlog.Title,
                    CategorieId = newBlog.CategoryId,
                    UserId = newBlog.UserId,
                    CreationDate=DateTime.Now
                    // User = user
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


            catch
            {

                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "bad request in the catch",

                });
            }


        }

        // delete a blog by its id 
        [HttpPost]
        [Route("/deleteBlog")]
        public async Task<IActionResult> Delete(@string id)
        {
            Guid guidId = Guid.Parse(id.name);
            var blog = await _DbBlogsContext.Blogs.FindAsync(guidId);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound(); // mbe3id bech inredirectiha lel api necessary 
            }
            _DbBlogsContext.Remove(blog);
            await _DbBlogsContext.SaveChangesAsync();
            return Ok();


        }


        [HttpPost]
        [Route("/addView")]
        public async Task<IActionResult> addView(@string id)
        {
            Guid guidId = Guid.Parse(id.name);
            var blog = await _DbBlogsContext.Blogs.FindAsync(guidId);

            // si le blog Id n'existe pas
            if (blog == null)
            {
                return NotFound(); // mbe3id bech inredirectiha lel api necessary 
            }

            blog.Views++;
            await _DbBlogsContext.SaveChangesAsync();
            return Ok();


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
            if (updatedBlog.CategorieId != null)
            {
                blog.CategorieId = updatedBlog.CategorieId;
            }

            await _DbBlogsContext.SaveChangesAsync();

            return blog;

        }







        [HttpPut]
        [Route("/likeBlog")]
        public async Task<IActionResult> likeBlog([FromBody] LikingRequest likeRequest)
        {
            Guid blogId = Guid.Parse(likeRequest.EntityId);
            Guid UserId = Guid.Parse(likeRequest.UserId);


            var blog = await _DbBlogsContext.Blogs.FindAsync(blogId);

            if (blog == null)
            {
                return NotFound();
            }


            var like = await _DbBlogsContext.BlogLikes.FirstOrDefaultAsync(x => x.BlogId == blogId
            && x.UserId== UserId);

            if (like == null)
            {
                var BlogLike = new BlogLikes()
                {
                    BlogId = blogId,
                    UserId = UserId,
                    // User = user
                };
                await _DbBlogsContext.BlogLikes.AddAsync(BlogLike);

                blog.Likes++;


                await _DbBlogsContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "like"
                });
            }
            else
            {

                _DbBlogsContext.BlogLikes.Remove(like);
                blog.Likes--;

                await _DbBlogsContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "unlike "
                });

            }



        }


        [HttpPut]
        [Route("/dislikeBlog")]
        public async Task<IActionResult> dislikeBlog([FromBody] LikingRequest likeRequest)
        {
           
                Guid blogId = Guid.Parse(likeRequest.EntityId);
                Guid UserId = Guid.Parse(likeRequest.UserId);


                var blog = await _DbBlogsContext.Blogs.FindAsync(blogId);

                if (blog == null)
                {
                    return NotFound();
                }


                var dislike = await _DbBlogsContext.BlogDislikes.FirstOrDefaultAsync(x => x.BlogId == blogId
                && x.UserId == UserId);

                if (dislike == null)
                {
                    var BlogDislike = new BlogDislikes()
                    {
                        BlogId = blogId,
                        UserId = UserId,
                    };
                    await _DbBlogsContext.BlogDislikes.AddAsync(BlogDislike);

                    blog.Dislikes++;


                    await _DbBlogsContext.SaveChangesAsync();

                    return Ok(new
                    {
                        Message = "dislike"
                    });
                }
                else
                {

                    _DbBlogsContext.BlogDislikes.Remove(dislike);
                    blog.Dislikes--;

                    await _DbBlogsContext.SaveChangesAsync();

                    return Ok(new
                    {
                        Message = "undislike "
                    });

                }



            }



        [HttpPost]
        [Route("/getBlogLikes")]
        public async Task<IActionResult> getBlogLikes([FromBody] LikingRequest likeRequest)
        {

            Guid blogId = Guid.Parse(likeRequest.EntityId);
            Guid UserId = Guid.Parse(likeRequest.UserId);


            var blog = await _DbBlogsContext.Blogs.FindAsync(blogId);

            if (blog == null)
            {
                return NotFound();
            }

            var like = await _DbBlogsContext.BlogLikes.FirstOrDefaultAsync(x => x.BlogId == blogId
            && x.UserId == UserId);

            var dislike = await _DbBlogsContext.BlogDislikes.FirstOrDefaultAsync(x => x.BlogId == blogId
            && x.UserId == UserId);

            return Ok(new
            {
likes=(like!=null),
dislikes=(dislike!=null),

            });

        }






        [HttpPost]
        [Route("/getBlogsByCategory")]
        public async Task<IActionResult> getBlogsByCategory(@string category)
        {
            int catId = int.Parse(category.name);

            IEnumerable<Blog> blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.CategorieId == catId);

            return Ok(blogs);

        }





        [HttpPost]
        [Route("/getBlogsByCategoryName")]
        public async Task<IActionResult> getBlogsByCategoryName(@string category)
        {
            var cat = await _DbBlogsContext.Categories.FirstOrDefaultAsync(x => x.Name == category.name);
            if (cat == null)
            {
                return NotFound();
            }

            IEnumerable<Blog> blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.CategorieId == cat.Id);

            return Ok(blogs);

        }




        // image management : 

        // upload a image (you can use it when creating the blog or when you wanna update the image 
        [HttpPost]
        [Route("ImageUpload")]
        public async Task<IActionResult> ImageUpload([FromForm] ImageModel imageModel)
        {

            var blog = await _DbBlogsContext.Blogs.FindAsync(imageModel.BlogId);

            if (blog == null)
            {
                return NotFound("Blog to insert the image not found ! ");
            }
            try
            {
                string Filepath = this._environment.WebRootPath + "\\Uploads\\Blogs\\" + imageModel.BlogId;
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }
                string imagepath = Filepath + "\\image.png";
                if (System.IO.File.Exists(imagepath))  // so you can use this also when you wanna update the image 
                {
                    System.IO.File.Delete(imagepath);
                }

                using (Stream stream = new FileStream(imagepath, FileMode.Create))
                {
                    imageModel.Image.CopyTo(stream);
                }

                blog.Image = getImageByBlog(imageModel.BlogId);
                _DbBlogsContext.SaveChanges();

                return Ok(blog);

            }
            catch (Exception ex)
            {
                return BadRequest("error in uploading the image !" + ex.Message);
            }

        }


        // to delete an image : 
        [HttpDelete]
        [Route("ImageRemove")]

        public async Task<IActionResult> removeImage(Guid blogId)
        {
            string Filepath = this._environment.WebRootPath + "\\Uploads\\Blogs\\" + blogId;
            string imagepath = Filepath + "\\image.png";

            if (System.IO.Directory.Exists(Filepath))
            {
                System.IO.File.Delete(imagepath);
                System.IO.Directory.Delete(Filepath);

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }


        // this action to let the attribue image in blog  an URL 
        [NonAction]
        private string getImageByBlog(Guid blogId)
        {
            string imageURL = string.Empty;
            string HostURL = "https://localhost:7054/"; // change it in your code  
            string Filepath = this._environment.WebRootPath + "\\Uploads\\Blogs\\" + blogId;
            string imagepath = Filepath + "\\image.png";

            if (!System.IO.Directory.Exists(Filepath))
            {
                imageURL = HostURL + "commun/noImage.jpg";
            }
            else
            {
                imageURL = HostURL + "Uploads/Blogs/" + blogId + "/image.png";
            }

            return imageURL;
        }


    }

}