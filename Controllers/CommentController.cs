using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_back_end.Data;
using Project_back_end.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.Design;

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
            string userIdString = newComment.UserId.ToString();

            var user = await _userManager.FindByIdAsync(userIdString);
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
                UserId = newComment.UserId.ToString(),
                Author = user,
                BlogId = newComment.BlogId,
                AssociatedBlog = blog,
                creationDate = DateTime.Now,    
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




        [HttpPost]
        [Route("/getCommentById")]
        public async Task<IActionResult> getCommentById(testModel id)
        {

            Guid guidId = Guid.Parse(id.name);

            var comment = await _db.Comments.FirstOrDefaultAsync(x=>x.Id==guidId);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
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

        [HttpPut]
        [Route("/likeComment/{id}")]
        public async Task<IActionResult> LikeComment([FromRoute] Guid id)
        {
            var comment = await _db.Comments.FindAsync(id);


            if (comment == null)
            {
                return NotFound("no comment to begin with");
            }


            comment.Likes ++;

            await _db.SaveChangesAsync();

            return Ok(comment);

        }
        [HttpPut]
        [Route("/dislikeComment/{id}")]
        public async Task<IActionResult> DislikeComment([FromRoute] Guid id)
        {
            var comment = await _db.Comments.FindAsync(id);


            if (comment == null)
            {
                return NotFound("no comment to begin with");
            }


            comment.Dislikes++;

            await _db.SaveChangesAsync();

            return Ok(comment);

        }


        [HttpPut]
        [Route("/likeComment")]
        public async Task<IActionResult> likeComment([FromBody] LikingRequest likeRequest)
        {

            Guid commentId = Guid.Parse(likeRequest.EntityId);
            Guid UserId = Guid.Parse(likeRequest.UserId);

             var comment = await _db.Comments.FirstOrDefaultAsync(x=> x.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

             
            var commentLike = await _db.CommentLikes.FirstOrDefaultAsync(x => x.CommentId == commentId
            && x.UserId == UserId);

            if (commentLike == null)
            {
                var CommentLike = new CommentLikes()
                {
                    CommentId = commentId,
                    UserId = UserId,
                };
                await _db.CommentLikes.AddAsync(CommentLike);

                comment.Likes++;


                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Message = "like"
                });
            }
            else
            {

                _db.CommentLikes.Remove(commentLike);
                comment.Likes--;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Message = "unlike "
                });

            }



        }


        [HttpPut]
        [Route("/dislikeComment")]
        public async Task<IActionResult> dislikeComment([FromBody] LikingRequest likeRequest)
        {

             Guid commentId = Guid.Parse(likeRequest.EntityId);
             Guid UserId = Guid.Parse(likeRequest.UserId);
          

            var comment = await _db.Comments.FirstOrDefaultAsync(x=>x.Id== commentId);

            if (comment == null)
            {
                return NotFound();
            }


            var dislikedcomment = await _db.CommentDislikes.FirstOrDefaultAsync(x => x.CommentId == commentId
            && x.UserId == UserId);

            if (dislikedcomment == null)
            {
                var CommentDislike = new CommentDislikes()
                {
                    CommentId = commentId,
                    UserId = UserId,
                };
                await _db.CommentDislikes.AddAsync(CommentDislike);

                comment.Dislikes++;


                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Message = "dislike"
                });
            }
            else
            {

                _db.CommentDislikes.Remove(dislikedcomment);
                comment.Dislikes--;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Message = "undislike "
                });

            }



        }


        [HttpPost]
        [Route("/getCommentLikes")]
        public async Task<IActionResult> getCommentLikes([FromBody] LikingRequest likeRequest)
        {
            Guid commentId = Guid.Parse(likeRequest.EntityId);
            Guid UserId = Guid.Parse(likeRequest.UserId);

            var comment = await _db.Comments.FirstOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            var like = await _db.CommentLikes.FirstOrDefaultAsync(x => x.CommentId == commentId
            && x.UserId == UserId);

            var dislike = await _db.CommentDislikes.FirstOrDefaultAsync(x => x.CommentId == commentId
            && x.UserId == UserId);

            return Ok(new
            {
                likes = (like != null),
                dislikes = (dislike != null),
            });

        }





      
        [HttpPost]
        [Route("/GetCommentsByBlog")]
        public async Task<IActionResult> GetCommentsByBlogs([FromBody] testModel Blogid)
        {
            Guid guid = Guid.Parse(Blogid.name);

            var comments =await  _db.Comments.Where(c => c.BlogId == guid)
                    .OrderByDescending(c => c.creationDate) // Order by the Date property in descending order
.ToListAsync();
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
