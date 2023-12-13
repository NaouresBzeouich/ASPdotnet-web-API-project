using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_back_end.Data;
using Project_back_end.Models;

namespace Project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly BlogsAPIDbContext _dbContext;

        public CategoriesController(BlogsAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // this is only to add some categories hawka 7outt eeli 7achtik bih be3id delete it 
        [HttpPost]
        [Route("/addCategories")]
        public async Task<IEnumerable<Categorie>> AddCategories()
        {
            // Create new categories
            var category1 = new Categorie { Name = "Category 1" };
            var category2 = new Categorie { Name = "Category 2" };

            // Add categories to the context
            _dbContext.Categories.Add(category1);
            _dbContext.Categories.Add(category2);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return _dbContext.Categories.ToList(); 

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
        [HttpGet]
        [Route("/getCategoryByName/{Name}")]
        public async Task<Categorie> getCategoryByName([FromRoute]string Name)
        {
            var categorie = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == Name);
            if(categorie == null)
            {
                return null; // change it if you have another case when there's no category with that name
            }
            
            return categorie;
        }



    }
}
