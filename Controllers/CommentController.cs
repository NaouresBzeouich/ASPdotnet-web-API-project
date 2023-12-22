using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly BlogsAPIDbContext _db;
        private readonly UserManager<User> _userManager;
        public static CreateCommentRequest com = new CreateCommentRequest();


        public CommentController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, BlogsAPIDbContext dbContext)
        {
            _userManager = userManager;
            
            _db = dbContext;
        }
        [HttpPost]
        [Route("/createComment")]

        public async Task<IActionResult> Create([FromBody] CreateCommentRequest newComment)
        {
            var user = await _userManager.FindByIdAsync(newComment.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var blog = await _db.Blogs.FindAsync(Guid.Parse(newComment.BlogId));
            if (blog == null)
            {
                return NotFound("Blog not found");
            }
            var Comment = new Comment()
            {
                Content = newComment.Content,
                UserId = newComment.UserId,
                Author = user,
                BlogId = Guid.Parse(newComment.BlogId),
                AssociatedBlog = blog,
            };
          await  _db.Comments.AddAsync(Comment);

            await _db.SaveChangesAsync();

            return Ok(Comment);

        }
        [HttpGet]
        [Route("/AllComments")]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _db.Comments.ToListAsync();
            return Ok(comments);
        }
        [HttpPut]
        [Route("/updateComment/{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] Guid id, string newContent)
        {
            var comment = await _db.Comments.FindAsync(id);

            
            if (comment == null)
            {
                return NotFound("no comment to begin with");  
            }

            
            comment.Content = newContent;

            await _db.SaveChangesAsync();

            return Ok(comment);

        }
        [HttpPost]

        public async Task<IActionResult> Index()
        {
            int a = 5;
            return Ok(a);
        }
        [HttpPost]
        [Route("/GetCommentsByBlog")]
        public IActionResult GetCommentsByBlogs([FromBody] testModel Blogid)
        {
            Guid guid = Guid.Parse(Blogid.name);

            var comments = _db.Comments.Where(c => c.BlogId == guid).ToList();
            return Ok(comments);
        }
        [HttpDelete]
        [Route("/DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null) {
                return NotFound("there isn't a comment to begin with");
            }
            _db.Comments.Remove(comment);
           await  _db.SaveChangesAsync();
            return Ok(comment);
        }


    }
    
}
