using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly BlogsAPIDbContext _db;
        public static CreateCommentRequest com = new CreateCommentRequest();
        

        public CommentController(BlogsAPIDbContext blogsAPIDbContext)
        {
            _db = blogsAPIDbContext;
        }
        [HttpPost]
        [Route("/createComment")]

        public async Task<IActionResult> Create([FromBody] CreateCommentRequest newComment)
        {
            var user = await _db.users.FindAsync(newComment.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var blog = await _db.Blogs.FindAsync(newComment.BlogId);
            if (blog == null)
            {
                return NotFound("Blog not found");
            }
            var Comment = new Comment()
            {
                Content = newComment.Content,
                //UserId = newComment.UserId,
                User = user,
                //BlogId = newComment.BlogId,
                Blog = blog,
            };
            await _db.Comments.AddAsync(Comment);

            await _db.SaveChangesAsync();

            return Ok(Comment);

        }
        [HttpPost]
       
        public async Task<IActionResult> Index()
        {
            int a = 5;
            return Ok();
        }


    }
}
