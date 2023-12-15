using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_back_end.Data;
using Project_back_end.Models;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly BlogsAPIDbContext _dbContext;

        public CategoryController(BlogsAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // this is only to add some categories hawka 7outt eeli 7achtik bih be3id delete it 
        [HttpPost]
        [Route("/addCategory")]
        public async Task<IActionResult> AddCategory(  testModel cat)
        {
            // Create new categories
            var category1 = new Categorie { Name = cat.name };

            // Add categories to the context
            _dbContext.Categories.Add(category1);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return Ok(new {
                StatusCode=200,
                    Message= "category added"
            }); 

        }

            // get all the categories
        [HttpGet]
        [Route("/getCategories")]
        public async Task<IEnumerable<Categorie>> getCategories()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return categories ;   
        }

        // get a category with its name 

        [HttpPost]
        [Route("/getCategoryByName")]
        public async Task<IActionResult> getCategoryByName(testModel cat)
        {
            var categorie = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name==cat.name);
            if (categorie == null)
            {
                return NotFound(new
                {
                    Message = "category not found",
                    namepassedby=cat.name
                }) ; // change it if you have another case when there's no category with that name
            }
            
            return Ok(new
            {
                id= categorie.Id,
                name= categorie.Name,
                StatusCode=200,

            });
        }



    }
}
