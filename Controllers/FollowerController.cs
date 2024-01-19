using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_back_end.Data;
using Project_back_end.Models;
using System.Linq;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowerController : ControllerBase
    {

        private readonly ILogger<BlogController> _logger;

        private readonly BlogsAPIDbContext _Db;

        private readonly IWebHostEnvironment _environment;

        public FollowerController(BlogsAPIDbContext DbBlogsContext, ILogger<BlogController> logger, IWebHostEnvironment _environment)
        {
            this._logger = logger;
            this._Db = DbBlogsContext;
            this._environment = _environment;
        }


        [HttpGet]
        [Route("/getFollowers")]
        public async Task<IActionResult> getFollowers()
        {
            var followers = await _Db.Followers.ToListAsync();
         
                return Ok(followers);
        }



        [HttpPost]
        [Route("/getFollowersById")]
        public async Task<IActionResult> getFollowersById(testModel id)
        {
            var followers = await _Db.Followers
                   .Where(x => x.FollowingId == id.name)
                   .ToListAsync();
         //   var followersCount = followers.Count;


            return Ok(followers
             );
        }




        [HttpPost]
        [Route("/getFollowingsById")]
        public async Task<IActionResult> getFollowingsById(testModel id)
        {

            var followings = await _Db.Followers
                   .Where(x => x.FollowerId == id.name)
                   .ToListAsync();

           // var followersCount = followings.Count;


            return Ok(
                 followings

            );
        }



        [HttpPut]
        [Route("/Follow")]
        public async Task<IActionResult> Follow(LikingRequest id)
        {
           
            var follower = await _Db.Users.FirstOrDefaultAsync(x=>x.Id==id.EntityId);
            var following = await _Db.Users.FirstOrDefaultAsync(x => x.Id == id.UserId);

            if ((follower == null)|| (following == null)){
                return NotFound();

            }

            var connection = new followers()
            {
                FollowerId = id.EntityId,
                FollowerUsername = follower.UserName,
                FollowingUsername = following.UserName,
                FollowingId = id.UserId,
            };

            await _Db.Followers.AddAsync(connection);
            await _Db.SaveChangesAsync();  


            return Ok(connection);
        }




        [HttpPut]
        [Route("/Unfollow")]
        public async Task<IActionResult> Unfollow(LikingRequest id)
        {

            var follower = await _Db.Users.FirstOrDefaultAsync(x => x.Id == id.EntityId);
            var following = await _Db.Users.FirstOrDefaultAsync(x => x.Id == id.UserId);

            if ((follower == null) || (following == null))
            {
                return NotFound();

            }
            var connection=await _Db.Followers.FirstOrDefaultAsync(x=>x.FollowerId==follower.Id 
            &&  x.FollowingId==following.Id);
            if (connection == null)
            {
                return NotFound();

            }

             _Db.Followers.Remove(connection);
            await _Db.SaveChangesAsync();


            return Ok(connection);
        }











    }
}
