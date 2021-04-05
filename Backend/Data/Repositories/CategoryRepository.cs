using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public new IEnumerable<Category> GetAll()
        {
            return _context.Set<Category>()
                           .Include(c => c.Questions)
                           .ToList();
        }

        public new Category GetById(int id)
        {
            return _context.Set<Category>()
                           .Include(c => c.Questions)
                           .Where(c => c.Id == id)
                           .FirstOrDefault();
        }

        public void AddCategory(string categoryName)
        {
            Add(new Category
            {
                Name = categoryName
            });
        }

        public new void Remove(Category entity)
        {
            _context.Set<Category>().Remove(entity);
        }

        public new void RemoveRange(IEnumerable<Category> entities)
        {
            _context.Set<Category>().RemoveRange(entities);
        }
    }
}
