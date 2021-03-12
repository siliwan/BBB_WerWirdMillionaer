using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(DatabaseContext context)
            : base(context) 
        { }

        public void AddCategory(string categoryName)
        {
            Add(new Category
            {
                Name = categoryName
            });
        }
    }
}
