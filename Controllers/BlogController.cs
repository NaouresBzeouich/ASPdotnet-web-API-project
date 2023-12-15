using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;

        private readonly BlogsAPIDbContext _DbBlogsContext;
        private readonly IWebHostEnvironment _environment; 
        public BlogController(BlogsAPIDbContext DbBlogsContext, IWebHostEnvironment _environment)

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
            if( blogs != null && blogs.Count >0)
            {
                foreach(var blog in blogs)
                {
                    blog.Image = getImageByBlog(blog.Id);
                }

                await _DbBlogsContext.SaveChangesAsync();

                return blogs;
            }else
            {
                return new List<Blog>();
            }
            
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

            blog.Image = getImageByBlog(blog.Id); 
            await _DbBlogsContext.SaveChangesAsync();

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

            var Blog = new Blog()
            {
                Content = newBlog.Content,
                Title = newBlog.Title,
                CategorieId = newBlog.CategorieId,
                UserId = newBlog.UserId,
                User = user
            };
            await _DbBlogsContext.Blogs.AddAsync(Blog);


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
            if (updatedBlog.CategorieId != null)
            {
                blog.CategorieId = updatedBlog.CategorieId;
            }

            await _DbBlogsContext.SaveChangesAsync();

            return Ok(blog);

        }

        // get blogs by its owner id 
        [HttpPost]
        [Route("/getBlogsByUser")]
        public async Task<IEnumerable<Blog>> getBlogsByUser([FromBody] testModel userId)
        {

            Guid id = Guid.Parse(userId.name);
            var blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.UserId == id);

            if (blogs == null)
            { return NotFound("there 's no blogs created by this user "); }
            return Ok(blogs);
        }


        // get blogs by category
        [HttpPost]
        [Route("/getBlogsByCategory")]
        public async Task<IEnumerable<Blog>> getBlogsByCategory( Categorie Category)
        {
            IEnumerable<Blog> blogs = _DbBlogsContext.Blogs.Where(Blog => Blog.CategorieId == Category.Id);
            if (blogs == null)
                return NotFound("there's no blogs in this category ");
            return Ok(blogs);

        }

        // image management : 

            // upload a image (you can use it when creating the blog or when you wanna update the image 
        [HttpPost]
        [Route("ImageUpload")]
        public async Task<IActionResult> ImageUpload([FromForm] ImageModel imageModel )
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

        [HttpDelete]
        [Route("ImageRemove")]

        public async   Task<IActionResult> removeImage(Guid blogId)
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
                imageURL = HostURL+"Uploads/Blogs/"+blogId+"/image.png";
            }

            return imageURL; 
        }
       

    }


}
