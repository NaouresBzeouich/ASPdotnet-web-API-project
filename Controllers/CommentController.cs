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

        public CommentController(BlogsAPIDbContext blogsAPIDbContext)
        {
            _db = blogsAPIDbContext;
        }

        
    }
}
