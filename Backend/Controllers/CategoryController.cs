using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> logger;
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(
                ILogger<CategoryController> logger,
                ICategoryRepository categoryRepository)
        {
            this.logger = logger;
            this.categoryRepository = categoryRepository;
        }

        // GET api/<CategoryController>
        [HttpGet]
        public IEnumerable<Category> Get()
        {
            return categoryRepository.GetAll();
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public ActionResult<Category> Get(int id)
        {
            var Category = categoryRepository.GetById(id);

            return Category != null ? Category : BadRequest($"Category with id {id} does not exist.");
        }

        // POST api/<CategoryController>
        [HttpPost]
        public async Task Post([FromBody] string value)
        {
            categoryRepository.AddCategory(value);
            await categoryRepository.SaveChangesAsync();
        }

        // PUT api/<CategoryController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] string value)
        {
            var CategoryToModify = categoryRepository.GetById(id);

            if (CategoryToModify != null)
            {
                CategoryToModify.Name = value;
                await categoryRepository.SaveChangesAsync();
                Ok();
            }
            else
            {
                NotFound($"Category with id {id} does not exist.");
            }
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var CategoryToRemove = categoryRepository.GetById(id);

            if (CategoryToRemove != null)
            {
                categoryRepository.Remove(CategoryToRemove);
                await categoryRepository.SaveChangesAsync();
                NoContent();
            }
            else
            {
                BadRequest($"Category with id {id} does not exist.");
            }
        }
    }
}
